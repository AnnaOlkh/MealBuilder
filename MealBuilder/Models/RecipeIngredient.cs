using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MealBuilder.Models;
public enum Unit
{
    Gram,
    Milliliter
}
public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = default!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = default!;

    [Range(0.0, 100000.0)]
    [Column(TypeName = "numeric(12,2)")]
    public decimal Quantity { get; set; }

    [Required]
    public Unit Unit { get; set; } = Unit.Gram;
}
