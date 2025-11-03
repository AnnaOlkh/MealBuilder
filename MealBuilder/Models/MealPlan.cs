using System.ComponentModel.DataAnnotations;

namespace MealBuilder.Models;

public class MealPlan
{
    public int Id { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = "My meal plan for this week";
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = default!;
    public ICollection<MealPlanRecipe> MealPlanRecipes { get; set; } = new List<MealPlanRecipe>();
}
