namespace MealBuilder.Models;

public class AppUser
{
    public int Id { get; set; }

    public string Provider { get; set; } = default!;
    public string ProviderUserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Name { get; set; }
    public bool IsPremium { get; set; }

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
}
