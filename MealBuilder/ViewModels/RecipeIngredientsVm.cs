using MealBuilder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MealBuilder.ViewModels;

public sealed class RecipeIngredientsVm
{
    public int RecipeId { get; set; }
    public string RecipeTitle { get; set; } = default!;

    public List<Row> Items { get; set; } = new();
    public List<SelectListItem> IngredientOptions { get; set; } = new();

    public AddDto Add { get; set; } = new();

    public sealed class Row
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = default!;
        public decimal Quantity { get; set; }
        public Unit Unit { get; set; }
    }

    public sealed class AddDto
    {
        [Required]
        public int IngredientId { get; set; }

        [Range(0.01, 100000)]
        public decimal Quantity { get; set; }

        [Required]
        public Unit Unit { get; set; } = Unit.Gram;

        [Required]
        public int RecipeId { get; set; }
    }
    public sealed class UpdateNotesDto
    {
        public int MealPlanId { get; set; }
        public int MealPlanRecipeId { get; set; }
        public string? Notes { get; set; }
    }
}
