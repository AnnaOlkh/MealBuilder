using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace MealBuilder.Models;
public enum Unit
{
    Gram,
    Milliliter,
    Piece
}
public class RecipeIngredient
{
    public int RecipeId { get; set; }
    [ValidateNever][BindNever][JsonIgnore]
    public Recipe Recipe { get; set; } = default!;

    public int IngredientId { get; set; }
    [ValidateNever][BindNever][JsonIgnore]
    public Ingredient Ingredient { get; set; } = default!;

    [Range(0.0, 100000.0)]
    [Column(TypeName = "numeric(12,2)")]
    public decimal Quantity { get; set; }

    [Required]
    public Unit Unit { get; set; } = Unit.Gram;
}
