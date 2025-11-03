using System.ComponentModel.DataAnnotations;

namespace MealBuilder.Models;
public enum MealCategory
{
    Breakfast,
    Dinner,
    Lunch,
    Dessert,
    Snack
}
public class Recipe
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = default!;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public MealCategory Category { get; set; }

    [Range(0, 100000)]
    public int? Calories { get; set; }

    [Url, Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }
    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<MealPlanRecipe> MealPlanRecipes { get; set; } = new List<MealPlanRecipe>();
}
