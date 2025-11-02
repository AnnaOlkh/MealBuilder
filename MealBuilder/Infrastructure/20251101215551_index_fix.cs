using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealBuilder.Infrastructure
{
    /// <inheritdoc />
    public partial class index_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MealPlanRecipes_MealPlanId_Day_MealType",
                table: "MealPlanRecipes");

            migrationBuilder.DropIndex(
                name: "IX_MealPlanRecipes_MealPlanId_Day_MealType_RecipeId",
                table: "MealPlanRecipes");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_MealPlanId_Day_MealType",
                table: "MealPlanRecipes",
                columns: new[] { "MealPlanId", "Day", "MealType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MealPlanRecipes_MealPlanId_Day_MealType",
                table: "MealPlanRecipes");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_MealPlanId_Day_MealType",
                table: "MealPlanRecipes",
                columns: new[] { "MealPlanId", "Day", "MealType" });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_MealPlanId_Day_MealType_RecipeId",
                table: "MealPlanRecipes",
                columns: new[] { "MealPlanId", "Day", "MealType", "RecipeId" },
                unique: true);
        }
    }
}
