using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HikeRecommendationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCareerPathTipsToHikeRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "ProjectsHandled",
                table: "PerformanceData",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "CareerPathTips",
                table: "HikeRecommendations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MarketComparison",
                table: "HikeRecommendations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PerformanceSummary",
                table: "HikeRecommendations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkillsGap",
                table: "HikeRecommendations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CareerPathTips",
                table: "HikeRecommendations");

            migrationBuilder.DropColumn(
                name: "MarketComparison",
                table: "HikeRecommendations");

            migrationBuilder.DropColumn(
                name: "PerformanceSummary",
                table: "HikeRecommendations");

            migrationBuilder.DropColumn(
                name: "SkillsGap",
                table: "HikeRecommendations");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectsHandled",
                table: "PerformanceData",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
