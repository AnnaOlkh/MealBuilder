using System.ComponentModel.DataAnnotations;

namespace MealBuilder.Models;

public class Ingredient
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = default!;
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
