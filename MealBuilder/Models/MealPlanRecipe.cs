using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MealBuilder.Models;
public enum MealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack,
    Dessert
}
public class MealPlanRecipe
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    [ValidateNever][BindNever][JsonIgnore]
    public MealPlan MealPlan { get; set; } = default!;

    public int RecipeId { get; set; }
    [ValidateNever][BindNever][JsonIgnore]
    public Recipe Recipe { get; set; } = default!;

    [Required]
    public DayOfWeek Day { get; set; }

    [Required]
    public MealType MealType { get; set; }

    [StringLength(300)]
    public string? Notes { get; set; }
}
