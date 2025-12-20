using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCenterManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAIRecommendationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "AIRecommendations");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AIRecommendations");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "AIRecommendations",
                newName: "RequestDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "AIRecommendations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "AIRecommendations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "AIRecommendations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AIRecommendations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceLevel",
                table: "AIRecommendations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AIRecommendations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "AIRecommendations");

            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                table: "AIRecommendations");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AIRecommendations");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "AIRecommendations",
                newName: "CreatedDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "AIRecommendations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "AIRecommendations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "AIRecommendations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "AIRecommendations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AIRecommendations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
