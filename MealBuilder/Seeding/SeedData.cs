using MealBuilder.Infrastructure;
using MealBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Seeding;

public class SeedData
{
    public static async Task EnsureSeededAsync(MealBuilderDbContext db)
    {
        if (await db.Recipes.AnyAsync()) return;
        var email = "an.olkhovska15@gmail.com";
        var demoUser = await db.AppUsers
            .FirstOrDefaultAsync(u => u.Provider == "Google" && u.Email == email);

        if (demoUser is null)
        {
            demoUser = new AppUser
            {
                Provider = "Google",
                ProviderUserId = "seed-placeholder",
                Email = email,
                Name = "Demo MealBuilder User"
            };
            db.AppUsers.Add(demoUser);
            await db.SaveChangesAsync();
        }

        // --- Ingredients (глобальні, без юзера) ---
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
            new() { Name = "Peanut Butter" },

            // --- New ingredients for extra recipes ---
            new() { Name = "Butter" },
            new() { Name = "Cocoa Powder" },
            new() { Name = "Baking Powder" },
            new() { Name = "Vanilla Extract" },
            new() { Name = "Apple" },
            new() { Name = "Cinnamon" },
            new() { Name = "Brown Sugar" },
            new() { Name = "Sweet Potato" },
            new() { Name = "Chickpeas" },
            new() { Name = "Tahini" },
            new() { Name = "Lemon" },
            new() { Name = "Spinach" },
            new() { Name = "Mozzarella" },
            new() { Name = "Balsamic Vinegar" },
            new() { Name = "Bell Pepper" },
            new() { Name = "Onion" },
            new() { Name = "Soy Sauce" },
            new() { Name = "Beef Sirloin" },
            new() { Name = "Broccoli" },
            new() { Name = "Carrot" },
            new() { Name = "Feta Cheese" },
            new() { Name = "Olives" },
            new() { Name = "Strawberries" },
            new() { Name = "Blueberries" },
            new() { Name = "Granola" },
            new() { Name = "Oregano" }
        };

        // --- Recipes (усі належать demoUser) ---
        var recipes = new List<Recipe>
        {
            new()
            {
                Title = "Classic Pancakes",
                Description = "Fluffy pancakes perfect for breakfast.",
                Category = MealCategory.Breakfast,
                Calories = 380,
                ImageUrl = "https://www.bhg.com/thmb/B1Mbx1q9AgIEJ8PbQpPq0QPs820=/4000x0/filters:no_upscale():strip_icc()/bhg-recipe-pancakes-waffles-pancakes-Hero-01-372c4cad318d4373b6288e993a60ca62.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Chicken Caesar Salad",
                Description = "Light salad with grilled chicken and parmesan.",
                Category = MealCategory.Lunch,
                Calories = 420,
                ImageUrl = "https://www.jessicagavin.com/wp-content/uploads/2022/06/chicken-caesar-salad-28-1200.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Spaghetti Pomodoro",
                Description = "Simple pasta with tomatoes, garlic, basil and olive oil.",
                Category = MealCategory.Dinner,
                Calories = 520,
                ImageUrl = "/images/pomodoro.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Grilled Salmon Bowl",
                Description = "Salmon over rice with cucumber and yogurt-lime sauce.",
                Category = MealCategory.Dinner,
                Calories = 610,
                ImageUrl = "/images/salmon-bowl.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Avocado Toast",
                Description = "Whole-grain toast with mashed avocado and lime.",
                Category = MealCategory.Snack,
                Calories = 320,
                ImageUrl = "/images/avocado-toast.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Overnight Oats",
                Description = "No-cook oats with yogurt, banana and honey.",
                Category = MealCategory.Breakfast,
                Calories = 350,
                ImageUrl = "/images/overnight-oats.jpg",
                AppUser = demoUser
            },

            // --- New recipes (10 more) ---
            new()
            {
                Title = "Berry Yogurt Parfait",
                Description = "Layers of yogurt, granola and mixed berries.",
                Category = MealCategory.Snack,
                Calories = 280,
                ImageUrl = "/images/berry-parfait.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Chocolate Mug Cake",
                Description = "Single-serve chocolate cake made in minutes.",
                Category = MealCategory.Snack,
                Calories = 390,
                ImageUrl = "/images/choco-mug-cake.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Apple Cinnamon Oat Crumble",
                Description = "Warm apples with oat-cinnamon topping.",
                Category = MealCategory.Snack,
                Calories = 360,
                ImageUrl = "/images/apple-crumble.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Hummus & Veggie Plate",
                Description = "Classic hummus with fresh crunchy vegetables.",
                Category = MealCategory.Lunch,
                Calories = 420,
                ImageUrl = "/images/hummus-plate.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Caprese Toast",
                Description = "Tomato, mozzarella and basil on toasted bread.",
                Category = MealCategory.Snack,
                Calories = 330,
                ImageUrl = "/images/caprese-toast.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Veggie Omelette",
                Description = "Egg omelette with spinach, onion and tomato.",
                Category = MealCategory.Breakfast,
                Calories = 310,
                ImageUrl = "/images/veggie-omelette.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Baked Sweet Potato",
                Description = "Oven-baked sweet potato with yogurt drizzle.",
                Category = MealCategory.Dinner,
                Calories = 400,
                ImageUrl = "/images/baked-sweet-potato.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Beef & Broccoli Stir-Fry",
                Description = "Quick stir-fry served over rice.",
                Category = MealCategory.Dinner,
                Calories = 650,
                ImageUrl = "/images/beef-broccoli.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Greek Salad",
                Description = "Tomato, cucumber, feta, olives and olive oil.",
                Category = MealCategory.Lunch,
                Calories = 360,
                ImageUrl = "/images/greek-salad.jpg",
                AppUser = demoUser
            },
            new()
            {
                Title = "Banana Peanut Smoothie",
                Description = "Creamy smoothie with banana, yogurt and peanut butter.",
                Category = MealCategory.Breakfast,
                Calories = 430,
                ImageUrl = "/images/banana-peanut-smoothie.jpg",
                AppUser = demoUser
            }
        };

        await db.Ingredients.AddRangeAsync(ingredients);
        await db.Recipes.AddRangeAsync(recipes);
        await db.SaveChangesAsync();

        var recipeByTitle = await db.Recipes.ToDictionaryAsync(r => r.Title, r => r.Id);
        var ingrByName = await db.Ingredients.ToDictionaryAsync(i => i.Name, i => i.Id);

        // --- RecipeIngredients (як було) ---
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
            new() { RecipeId = recipeByTitle["Overnight Oats"], IngredientId = ingrByName["Milk"], Quantity = 60.00m, Unit = Unit.Milliliter },

            // Berry Yogurt Parfait
            new() { RecipeId = recipeByTitle["Berry Yogurt Parfait"], IngredientId = ingrByName["Yogurt"], Quantity = 150.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Berry Yogurt Parfait"], IngredientId = ingrByName["Granola"], Quantity = 40.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Berry Yogurt Parfait"], IngredientId = ingrByName["Strawberries"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Berry Yogurt Parfait"], IngredientId = ingrByName["Blueberries"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Berry Yogurt Parfait"], IngredientId = ingrByName["Honey"], Quantity = 8.00m, Unit = Unit.Gram },

            // Chocolate Mug Cake
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Flour"], Quantity = 40.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Sugar"], Quantity = 25.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Cocoa Powder"], Quantity = 12.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Milk"], Quantity = 60.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Baking Powder"], Quantity = 3.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Butter"], Quantity = 10.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Chocolate Mug Cake"], IngredientId = ingrByName["Vanilla Extract"], Quantity = 2.00m, Unit = Unit.Gram },

            // Apple Cinnamon Oat Crumble
            new() { RecipeId = recipeByTitle["Apple Cinnamon Oat Crumble"], IngredientId = ingrByName["Apple"], Quantity = 200.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Apple Cinnamon Oat Crumble"], IngredientId = ingrByName["Oats"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Apple Cinnamon Oat Crumble"], IngredientId = ingrByName["Cinnamon"], Quantity = 2.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Apple Cinnamon Oat Crumble"], IngredientId = ingrByName["Brown Sugar"], Quantity = 20.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Apple Cinnamon Oat Crumble"], IngredientId = ingrByName["Butter"], Quantity = 20.00m, Unit = Unit.Gram },

            // Hummus & Veggie Plate
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Chickpeas"], Quantity = 240.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Tahini"], Quantity = 30.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Lemon"], Quantity = 10.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Olive Oil"], Quantity = 15.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Garlic"], Quantity = 4.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Cucumber"], Quantity = 80.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Carrot"], Quantity = 80.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Hummus & Veggie Plate"], IngredientId = ingrByName["Salt"], Quantity = 2.00m, Unit = Unit.Gram },

            // Caprese Toast
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Whole Grain Bread"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Tomato"], Quantity = 80.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Mozzarella"], Quantity = 70.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Basil"], Quantity = 3.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Olive Oil"], Quantity = 8.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Balsamic Vinegar"], Quantity = 5.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Salt"], Quantity = 1.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Caprese Toast"], IngredientId = ingrByName["Black Pepper"], Quantity = 0.50m, Unit = Unit.Gram },

            // Veggie Omelette
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Eggs"], Quantity = 3.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Spinach"], Quantity = 40.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Onion"], Quantity = 30.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Tomato"], Quantity = 60.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Olive Oil"], Quantity = 5.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Salt"], Quantity = 1.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Veggie Omelette"], IngredientId = ingrByName["Black Pepper"], Quantity = 0.50m, Unit = Unit.Gram },

            // Baked Sweet Potato
            new() { RecipeId = recipeByTitle["Baked Sweet Potato"], IngredientId = ingrByName["Sweet Potato"], Quantity = 300.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Baked Sweet Potato"], IngredientId = ingrByName["Yogurt"], Quantity = 40.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Baked Sweet Potato"], IngredientId = ingrByName["Honey"], Quantity = 8.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Baked Sweet Potato"], IngredientId = ingrByName["Butter"], Quantity = 10.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Baked Sweet Potato"], IngredientId = ingrByName["Salt"], Quantity = 1.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Baked Sweet Potato"], IngredientId = ingrByName["Black Pepper"], Quantity = 0.50m, Unit = Unit.Gram },

            // Beef & Broccoli Stir-Fry
            new() { RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], IngredientId = ingrByName["Beef Sirloin"], Quantity = 220.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], IngredientId = ingrByName["Broccoli"], Quantity = 180.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], IngredientId = ingrByName["Soy Sauce"], Quantity = 25.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], IngredientId = ingrByName["Garlic"], Quantity = 6.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], IngredientId = ingrByName["Olive Oil"], Quantity = 10.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], IngredientId = ingrByName["Rice"], Quantity = 180.00m, Unit = Unit.Gram },

            // Greek Salad
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Tomato"], Quantity = 200.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Cucumber"], Quantity = 150.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Feta Cheese"], Quantity = 80.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Olives"], Quantity = 40.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Olive Oil"], Quantity = 12.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Lemon"], Quantity = 8.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Oregano"], Quantity = 1.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Greek Salad"], IngredientId = ingrByName["Salt"], Quantity = 1.00m, Unit = Unit.Gram },

            // Banana Peanut Smoothie
            new() { RecipeId = recipeByTitle["Banana Peanut Smoothie"], IngredientId = ingrByName["Banana"], Quantity = 150.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Banana Peanut Smoothie"], IngredientId = ingrByName["Peanut Butter"], Quantity = 25.00m, Unit = Unit.Gram },
            new() { RecipeId = recipeByTitle["Banana Peanut Smoothie"], IngredientId = ingrByName["Yogurt"], Quantity = 120.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Banana Peanut Smoothie"], IngredientId = ingrByName["Milk"], Quantity = 120.00m, Unit = Unit.Milliliter },
            new() { RecipeId = recipeByTitle["Banana Peanut Smoothie"], IngredientId = ingrByName["Honey"], Quantity = 6.00m, Unit = Unit.Gram },
        };
        await db.RecipeIngredients.AddRangeAsync(ri);

        // --- MealPlan & Recipes (оригінальний план, належить demoUser) ---
        var plan = new MealPlan
        {
            Name = "My meal plan for 27.10-02.11 week",
            AppUser = demoUser
        };
        await db.MealPlans.AddAsync(plan);
        await db.SaveChangesAsync();

        var mealPlanId = plan.Id;

        var mpr = new List<MealPlanRecipe>
        {
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Classic Pancakes"],       Day = DayOfWeek.Monday,  MealType = MealType.Breakfast, Notes = "Add berries on top" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Chicken Caesar Salad"],   Day = DayOfWeek.Monday,  MealType = MealType.Lunch,     Notes = "Grill chicken beforehand" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Spaghetti Pomodoro"],     Day = DayOfWeek.Tuesday, MealType = MealType.Dinner,    Notes = "Use fresh basil" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Avocado Toast"],          Day = DayOfWeek.Wednesday,MealType = MealType.Snack,   Notes = "Toast bread well" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Overnight Oats"],         Day = DayOfWeek.Thursday,MealType = MealType.Breakfast,Notes = "Make it the night before" },
            new() { MealPlanId = mealPlanId, RecipeId = recipeByTitle["Grilled Salmon Bowl"],    Day = DayOfWeek.Friday,  MealType = MealType.Dinner,    Notes = "Don’t overcook salmon" }
        };
        await db.MealPlanRecipes.AddRangeAsync(mpr);

        // --- Second MealPlan (також demoUser) ---
        var plan2 = new MealPlan
        {
            Name = "Meal plan for 03.11-10.11",
            AppUser = demoUser
        };
        await db.MealPlans.AddAsync(plan2);
        await db.SaveChangesAsync();

        var mealPlanId2 = plan2.Id;

        var mpr2 = new List<MealPlanRecipe>
        {
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Veggie Omelette"],          Day = DayOfWeek.Monday,    MealType = MealType.Breakfast, Notes = "Season well" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Greek Salad"],              Day = DayOfWeek.Monday,    MealType = MealType.Lunch,     Notes = "Add extra olives" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], Day = DayOfWeek.Monday,    MealType = MealType.Dinner,    Notes = "Serve over rice" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Banana Peanut Smoothie"],   Day = DayOfWeek.Tuesday,  MealType = MealType.Breakfast, Notes = "Blend until creamy" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Hummus & Veggie Plate"],    Day = DayOfWeek.Tuesday,  MealType = MealType.Lunch,     Notes = "Use tahini generously" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Spaghetti Pomodoro"],       Day = DayOfWeek.Tuesday,  MealType = MealType.Dinner,    Notes = "Finish with olive oil" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Classic Pancakes"],         Day = DayOfWeek.Wednesday,MealType = MealType.Breakfast, Notes = "Top with berries" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Chicken Caesar Salad"],     Day = DayOfWeek.Wednesday,MealType = MealType.Lunch,     Notes = "Croutons optional" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Baked Sweet Potato"],       Day = DayOfWeek.Wednesday,MealType = MealType.Dinner,    Notes = "Fork-tender" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Overnight Oats"],           Day = DayOfWeek.Thursday, MealType = MealType.Breakfast, Notes = "Prep night before" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Caprese Toast"],            Day = DayOfWeek.Thursday, MealType = MealType.Snack,     Notes = "Splash of balsamic" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Grilled Salmon Bowl"],      Day = DayOfWeek.Thursday, MealType = MealType.Dinner,    Notes = "Crispy skin" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Berry Yogurt Parfait"],     Day = DayOfWeek.Friday,   MealType = MealType.Breakfast, Notes = "Use fresh berries" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Greek Salad"],              Day = DayOfWeek.Friday,   MealType = MealType.Lunch,     Notes = "Light lunch" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Beef & Broccoli Stir-Fry"], Day = DayOfWeek.Friday,   MealType = MealType.Dinner,    Notes = "Don’t overcook broccoli" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Veggie Omelette"],          Day = DayOfWeek.Saturday, MealType = MealType.Breakfast, Notes = "Fluffy eggs" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Hummus & Veggie Plate"],    Day = DayOfWeek.Saturday, MealType = MealType.Lunch,     Notes = "Extra lemon" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Baked Sweet Potato"],       Day = DayOfWeek.Saturday, MealType = MealType.Dinner,    Notes = "Great with yogurt" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Banana Peanut Smoothie"],   Day = DayOfWeek.Sunday,   MealType = MealType.Breakfast, Notes = "Add ice if needed" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Caprese Toast"],            Day = DayOfWeek.Sunday,   MealType = MealType.Snack,     Notes = "Toast bread well" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Chocolate Mug Cake"],       Day = DayOfWeek.Friday,   MealType = MealType.Snack,     Notes = "Microwave ~60–90s" },
            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Grilled Salmon Bowl"],      Day = DayOfWeek.Sunday,   MealType = MealType.Dinner,    Notes = "Finish with lime" },

            new() { MealPlanId = mealPlanId2, RecipeId = recipeByTitle["Apple Cinnamon Oat Crumble"], Day = DayOfWeek.Saturday, MealType = MealType.Snack, Notes = "Serve warm" }
        };
        await db.MealPlanRecipes.AddRangeAsync(mpr2);

        await db.SaveChangesAsync();
    }
}