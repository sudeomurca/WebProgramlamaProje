using System.Text;
using System.Text.Json;

namespace FitnessCenterManagement.Services
{
    public class CohereAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public CohereAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Cohere:ApiKey"] ?? throw new InvalidOperationException("Cohere API key bulunamadı");
        }

        public async Task<string> GetRecommendationAsync(string prompt)
        {
            try
            {
                
                var url = "https://api.cohere.ai/v1/chat";

                var requestBody = new
                {
                    message = prompt,
                    model = "command-r-08-2024",
                    temperature = 0.7
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"API Hatası ({response.StatusCode}): {errorContent}";
                }

                var responseString = await response.Content.ReadAsStringAsync();
                
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? "Öneri alınamadı";
                }

                return $"AI'dan yanıt alınamadı. Response: {responseString}";
            }
            catch (Exception ex)
            {
                return $"Hata oluştu: {ex.Message}";
            }
        }
    }
}
