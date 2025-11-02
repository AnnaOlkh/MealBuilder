using MealBuilder.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MealBuilder.ViewModels;

public sealed class MealPlanDetailsVm
{
    public int MealPlanId { get; set; }
    public string Name { get; set; } = default!;

    public List<SlotVm> Slots { get; set; } = new();
    public List<SelectListItem> RecipeOptions { get; set; } = new();
    public EditSlotDto Edit { get; set; } = new();

    public sealed class SlotVm
    {
        public DayOfWeek Day { get; set; }
        public MealType MealType { get; set; }

        public int? MealPlanRecipeId { get; set; }
        public int? RecipeId { get; set; }
        public string? RecipeTitle { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class EditSlotDto
    {
        [Required] public int MealPlanId { get; set; }
        [Required] public DayOfWeek Day { get; set; }
        [Required] public MealType MealType { get; set; }
        [Required] public int RecipeId { get; set; }
        [StringLength(300)] public string? Notes { get; set; }
    }
}