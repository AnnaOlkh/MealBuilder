using MealBuilder.Infrastructure;
using MealBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Seeding;

public class SeedData
{
    public static async Task EnsureSeededAsync(MealBuilderDbContext db)
    {
        if (await db.Recipes.AnyAsync()) return;

        // --- Ingredients ---
        var ingredients = new List<Ingredient>
        {
            new() { Name = "Flour" },
            new() { Name = "Milk" },
            new() { Name = "Eggs" },
            new() { Name = "Sugar" },
            new() { Name = "Parmesan" },
            new() { Name = "Lettuce" },
            new() { Name = "Chicken Breast" },
            new() { Name = "Tomato" },
            new() { Name = "Garlic" },
            new() { Name = "Basil" },
            new() { Name = "Olive Oil" },
            new() { Name = "Spaghetti" },
            new() { Name = "Salt" },
            new() { Name = "Black Pepper" },
            new() { Name = "Salmon Fillet" },
            new() { Name = "Rice" },
            new() { Name = "Avocado" },
            new() { Name = "Whole Grain Bread" },
            new() { Name = "Lime" },
            new() { Name = "Cucumber" },
            new() { Name = "Yogurt" },
            new() { Name = "Honey" },
            new() { Name = "Oats" },
            new() { Name = "Banana" },
            new() { Name = "Peanut Butter" }
        };

        // --- Recipes ---
        var recipes = new List<Recipe>
        {
            new()
            {
                Title = "Classic Pancakes",
                Description = "Fluffy pancakes perfect for breakfast.",
                Category = MealCategory.Breakfast,
                Calories = 380,
                ImageUrl = "https://www.bhg.com/thmb/B1Mbx1q9AgIEJ8PbQpPq0QPs820=/4000x0/filters:no_upscale():strip_icc()/bhg-recipe-pancakes-waffles-pancakes-Hero-01-372c4cad318d4373b6288e993a60ca62.jpg"
            },
            new()
            {
                Title = "Chicken Caesar Salad",
                Description = "Light salad with grilled chicken and parmesan.",
                Category = MealCategory.Lunch,
                Calories = 420,
                ImageUrl = "https://www.jessicagavin.com/wp-content/uploads/2022/06/chicken-caesar-salad-28-1200.jpg"
            },
            new()
            {
                Title = "Spaghetti Pomodoro",
                Description = "Simple pasta with tomatoes, garlic, basil and olive oil.",
                Category = MealCategory.Dinner,
                Calories = 520,
                ImageUrl = "/images/pomodoro.jpg"
            },
            new()
            {
                Title = "Grilled Salmon Bowl",
                Description = "Salmon over rice with cucumber and yogurt-lime sauce.",
                Category = MealCategory.Dinner,
                Calories = 610,
                ImageUrl = "/images/salmon-bowl.jpg"
            },
            new()
            {
                Title = "Avocado Toast",
                Description = "Whole-grain toast with mashed avocado and lime.",
                Category = MealCategory.Snack,
                Calories = 320,
                ImageUrl = "/images/avocado-toast.jpg"
            },
            new()
            {
                Title = "Overnight Oats",
                Description = "No-cook oats with yogurt, banana and honey.",
                Category = MealCategory.Breakfast,
                Calories = 350,
                ImageUrl = "/images/overnight-oats.jpg"
            }
        };

        await db.Ingredients.AddRangeAsync(ingredients);
        await db.Recipes.AddRangeAsync(recipes);
        await db.SaveChangesAsync();

        var recipeByTitle = await db.Recipes.ToDictionaryAsync(r => r.Title, r => r.Id);
        var ingrByName = await db.Ingredients.ToDictionaryAsync(i => i.Name, i => i.Id);

        // --- RecipeIngredients ---
        var ri = new List<RecipeIngredient>
        {
            // Classic Pancakes
            new() { RecipeId = recipeByTitle["Classic Pancakes"], IngredientId = ingrByName["Flour"], Quantity = 200.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Classic Pancakes"], IngredientId = ingrByName["Milk"], Quantity = 250.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Classic Pancakes"], IngredientId = ingrByName["Eggs"], Quantity = 2.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Classic Pancakes"], IngredientId = ingrByName["Sugar"], Quantity = 30.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Classic Pancakes"], IngredientId = ingrByName["Salt"], Quantity = 2.00m, Unit = Unit.Gram },

            // Chicken Caesar Salad
            new() { RecipeId = recipeByTitle["Chicken Caesar Salad"], IngredientId = ingrByName["Chicken Breast"], Quantity = 200.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chicken Caesar Salad"], IngredientId = ingrByName["Lettuce"], Quantity = 150.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chicken Caesar Salad"], IngredientId = ingrByName["Parmesan"], Quantity = 30.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chicken Caesar Salad"], IngredientId = ingrByName["Olive Oil"], Quantity = 10.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Chicken Caesar Salad"], IngredientId = ingrByName["Black Pepper"], Quantity = 1.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chicken Caesar Salad"], IngredientId = ingrByName["Salt"], Quantity = 2.00m, Unit = Unit.Gram },

            // Spaghetti Pomodoro
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Spaghetti"], Quantity = 180.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Tomato"], Quantity = 300.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Garlic"], Quantity = 6.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Basil"], Quantity = 5.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Olive Oil"], Quantity = 20.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Salt"], Quantity = 2.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Spaghetti Pomodoro"], IngredientId = ingrByName["Black Pepper"], Quantity = 1.00m, Unit = Unit.Gram },

            // Grilled Salmon Bowl
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Salmon Fillet"], Quantity = 220.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Rice"], Quantity = 180.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Cucumber"], Quantity = 80.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Yogurt"], Quantity = 100.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Lime"], Quantity = 10.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Olive Oil"], Quantity = 10.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Salt"], Quantity = 2.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Grilled Salmon Bowl"], IngredientId = ingrByName["Black Pepper"], Quantity = 1.00m, Unit = Unit.Gram },

            // Avocado Toast
            new() { RecipeId = recipeByTitle["Avocado Toast"], IngredientId = ingrByName["Whole Grain Bread"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Avocado Toast"], IngredientId = ingrByName["Avocado"], Quantity = 100.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Avocado Toast"], IngredientId = ingrByName["Lime"], Quantity = 5.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Avocado Toast"], IngredientId = ingrByName["Salt"], Quantity = 1.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Avocado Toast"], IngredientId = ingrByName["Black Pepper"], Quantity = 0.50m, Unit = Unit.Gram },

            // Overnight Oats
            new() { RecipeId = recipeByTitle["Overnight Oats"], IngredientId = ingrByName["Oats"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Overnight Oats"], IngredientId = ingrByName["Yogurt"], Quantity = 120.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Overnight Oats"], IngredientId = ingrByName["Banana"], Quantity = 120.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Overnight Oats"], IngredientId = ingrByName["Honey"], Quantity = 10.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Overnight Oats"], IngredientId = ingrByName["Milk"], Quantity = 60.00m, Unit = Unit.Milliliter }
        };
        await db.RecipeIngredients.AddRangeAsync(ri);

        // --- MealPlan & Recipes ---
        var plan = new MealPlan { Name = "My meal plan for this week" };
        await db.MealPlans.AddAsync(plan);
        await db.SaveChangesAsync();

        var mealPlanId = plan.Id;

        var mpr = new List<MealPlanRecipe>
        {
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Classic Pancakes"], Day = DayOfWeek.Monday,    MealType = MealType.Breakfast, Notes = "Add berries on top" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Chicken Caesar Salad"], Day = DayOfWeek.Monday,    MealType = MealType.Lunch,     Notes = "Grill chicken beforehand" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Spaghetti Pomodoro"], Day = DayOfWeek.Tuesday,   MealType = MealType.Dinner,    Notes = "Use fresh basil" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Avocado Toast"], Day = DayOfWeek.Wednesday, MealType = MealType.Snack,     Notes = "Toast bread well" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Overnight Oats"], Day = DayOfWeek.Thursday,  MealType = MealType.Breakfast, Notes = "Make it the night before" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Grilled Salmon Bowl"], Day = DayOfWeek.Friday,    MealType = MealType.Dinner,    Notes = "Don’t overcook salmon" }
        };
        await db.MealPlanRecipes.AddRangeAsync(mpr);

        await db.SaveChangesAsync();
    }
}
