using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MealBuilder.Infrastructure;
using MealBuilder.Models;
using MealBuilder.ViewModels;

namespace MealBuilder.Controllers
{
    public class RecipesController : Controller
    {
        private readonly MealBuilderDbContext _context;

        public RecipesController(MealBuilderDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ManageIngredients(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

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
            if (!ModelState.IsValid)
            {
                return await ManageIngredients(model.RecipeId);
            }

            var existsRecipe = await _context.Recipes.AnyAsync(r => r.Id == model.RecipeId);
            var existsIngredient = await _context.Ingredients.AnyAsync(i => i.Id == model.IngredientId);
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

        // POST: /Recipes/RemoveIngredient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveIngredient(int recipeId, int ingredientId)
        {
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
            return View(await _context.Recipes.ToListAsync());
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Category,Calories,ImageUrl")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,Calories,ImageUrl")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Landing(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

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
