namespace FitnessCenterManagement.Services
{
    public interface IAIService
    {
        Task<string> GetRecommendationAsync(string prompt);
    }
}
