using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealBuilder.Infrastructure;
using MealBuilder.Models;
using MealBuilder.Services;
using MealBuilder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Controllers
{
    [Authorize]
    public class RecipesController : AppControllerBase
    {
        private readonly IImageStorage _imageStorage;
        public RecipesController(MealBuilderDbContext context, IImageStorage imageStorage)
            : base(context)
        {
            _imageStorage = imageStorage;
        }

        public async Task<IActionResult> ManageIngredients(int id)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id && r.AppUserId == userId);

            if (recipe == null) return NotFound();

            var vm = new RecipeIngredientsVm
            {
                RecipeId = recipe.Id,
                RecipeTitle = recipe.Title,
                Items = recipe.RecipeIngredients
                    .OrderBy(ri => ri.Ingredient.Name)
                    .Select(ri => new RecipeIngredientsVm.Row
                    {
                        IngredientId = ri.IngredientId,
                        IngredientName = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    })
                    .ToList(),
                IngredientOptions = await _context.Ingredients
                    .OrderBy(i => i.Name)
                    .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Name })
                    .ToListAsync(),
                Add = new RecipeIngredientsVm.AddDto { RecipeId = recipe.Id }
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIngredient([Bind(Prefix = "Add")] RecipeIngredientsVm.AddDto model)
        {
            var userId = await GetCurrentAppUserIdAsync();

            if (!ModelState.IsValid)
            {
                return await ManageIngredients(model.RecipeId);
            }

            // рецепт має належати поточному юзеру
            var existsRecipe = await _context.Recipes
                .AnyAsync(r => r.Id == model.RecipeId && r.AppUserId == userId);

            var existsIngredient = await _context.Ingredients
                .AnyAsync(i => i.Id == model.IngredientId);

            if (!existsRecipe || !existsIngredient)
            {
                ModelState.AddModelError(string.Empty, "Recipe або Ingredient not found.");
                return await ManageIngredients(model.RecipeId);
            }

            var existing = await _context.RecipeIngredients
                .FindAsync(model.RecipeId, model.IngredientId);

            if (existing is null)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = model.RecipeId,
                    IngredientId = model.IngredientId,
                    Quantity = model.Quantity,
                    Unit = model.Unit
                });
            }
            else
            {
                existing.Quantity = model.Quantity;
                existing.Unit = model.Unit;
                _context.RecipeIngredients.Update(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageIngredients), new { id = model.RecipeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveIngredient(int recipeId, int ingredientId)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var ownsRecipe = await _context.Recipes
                .AnyAsync(r => r.Id == recipeId && r.AppUserId == userId);

            if (!ownsRecipe) return NotFound();

            var link = await _context.RecipeIngredients.FindAsync(recipeId, ingredientId);
            if (link != null)
            {
                _context.RecipeIngredients.Remove(link);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageIngredients), new { id = recipeId });
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentAppUserIdAsync();

            var recipes = await _context.Recipes
                .Where(r => r.AppUserId == userId)
                .ToListAsync();

            return View(recipes);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = await GetCurrentAppUserIdAsync();

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.Id == id && m.AppUserId == userId);

            if (recipe == null) return NotFound();

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,Calories,ImageUrl")] Recipe recipe,
            IFormFile? imageFile, CancellationToken ct)
        {
            var userId = await GetCurrentAppUserIdAsync();
            recipe.AppUserId = userId;
            ModelState.Remove(nameof(Recipe.AppUserId));
            if (!ModelState.IsValid)
            {
                return View(recipe);
            }
            if (imageFile is { Length: > 0 })
            {
                var imageUrl = await _imageStorage.UploadAsync(imageFile, ct);
                recipe.ImageUrl = imageUrl;
            }
            _context.Add(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = await GetCurrentAppUserIdAsync();

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.AppUserId == userId);

            if (recipe == null) return NotFound();

            return View(recipe);
        }

        // POST: Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,Calories")] Recipe recipe,
            IFormFile? imageFile, CancellationToken ct)
        {
            if (id != recipe.Id) return NotFound();
            ModelState.Remove(nameof(Recipe.AppUserId));
            if (!ModelState.IsValid)
            {
                return View(recipe);
            }

            var userId = await GetCurrentAppUserIdAsync();

            var existing = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.AppUserId == userId);

            if (existing == null) return NotFound();

            existing.Title = recipe.Title;
            existing.Description = recipe.Description;
            existing.Category = recipe.Category;
            existing.Calories = recipe.Calories;
            existing.ImageUrl = recipe.ImageUrl;
            if (imageFile is { Length: > 0 })
            {
                var imageUrl = await _imageStorage.UploadAsync(imageFile, ct);
                existing.ImageUrl = imageUrl;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Recipes.AnyAsync(e => e.Id == id && e.AppUserId == userId))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = await GetCurrentAppUserIdAsync();

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.Id == id && m.AppUserId == userId);

            if (recipe == null) return NotFound();

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.AppUserId == userId);

            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Landing(int id)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id && r.AppUserId == userId);

            if (recipe == null) return NotFound();

            var vm = new RecipeLandingVm
            {
                RecipeId = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Category = recipe.Category,
                Calories = recipe.Calories,
                ImageUrl = recipe.ImageUrl,
                Ingredients = recipe.RecipeIngredients
                    .OrderBy(ri => ri.Ingredient.Name)
                    .Select(ri => new RecipeLandingVm.IngredientRow
                    {
                        Name = ri.Ingredient.Name,
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    })
                    .ToList()
            };

            vm.UsedInMealPlansCount = await _context.MealPlanRecipes
                .CountAsync(m => m.RecipeId == recipe.Id);

            return View(vm);
        }
    }
}