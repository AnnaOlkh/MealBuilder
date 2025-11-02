using MealBuilder.Models;

namespace MealBuilder.ViewModels;

public class RecipeLandingVm
{
    public int RecipeId { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public MealCategory Category { get; set; }
    public int? Calories { get; set; }
    public string? ImageUrl { get; set; }
    public int UsedInMealPlansCount { get; set; }
    public List<IngredientRow> Ingredients { get; set; } = new();

    public class IngredientRow
    {
        public string Name { get; set; } = "";
        public decimal Quantity { get; set; }
        public Unit Unit { get; set; }
    }
}
