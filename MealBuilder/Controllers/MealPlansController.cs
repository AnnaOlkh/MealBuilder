using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealBuilder.Infrastructure;
using MealBuilder.Models;
using MealBuilder.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static MealBuilder.ViewModels.RecipeIngredientsVm;

namespace MealBuilder.Controllers
{
    [Authorize]
    public class MealPlansController : AppControllerBase
    {
        public MealPlansController(MealBuilderDbContext context)
            : base(context)
        {
        }

        // GET: MealPlans
        public async Task<IActionResult> Index()
        {
            var userId = await GetCurrentAppUserIdAsync();

            var plans = await _context.MealPlans
                .Where(p => p.AppUserId == userId)
                .ToListAsync();

            return View(plans);
        }

        // GET: MealPlans/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var plan = await _context.MealPlans
                .Include(p => p.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                .FirstOrDefaultAsync(p => p.Id == id && p.AppUserId == userId);

            if (plan == null) return NotFound();

            var allMealTypes = Enum.GetValues<MealType>();
            var slots = new List<MealPlanDetailsVm.SlotVm>();

            foreach (var day in Enum.GetValues<DayOfWeek>())
            {
                foreach (var mt in allMealTypes)
                {
                    var existing = plan.MealPlanRecipes
                        .FirstOrDefault(x => x.Day == day && x.MealType == mt);

                    slots.Add(new MealPlanDetailsVm.SlotVm
                    {
                        Day = day,
                        MealType = mt,
                        MealPlanRecipeId = existing?.Id,
                        RecipeId = existing?.RecipeId,
                        RecipeTitle = existing?.Recipe?.Title,
                        Notes = existing?.Notes,
                        Calories = existing?.Recipe?.Calories ?? 0
                    });
                }
            }

            var vm = new MealPlanDetailsVm
            {
                MealPlanId = plan.Id,
                Name = plan.Name,
                Slots = slots
                    .OrderBy(s => s.Day)
                    .ThenBy(s => s.MealType)
                    .ToList(),
                // показуємо тільки рецепти поточного юзера
                RecipeOptions = await _context.Recipes
                    .Where(r => r.AppUserId == userId)
                    .OrderBy(r => r.Title)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Title })
                    .ToListAsync(),
                Edit = new MealPlanDetailsVm.EditSlotDto { MealPlanId = plan.Id }
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind(Prefix = "Edit")] MealPlanDetailsVm.EditSlotDto dto)
        {
            var userId = await GetCurrentAppUserIdAsync();

            if (!ModelState.IsValid)
                return await Details(dto.MealPlanId);

            var planExists = await _context.MealPlans
                .AnyAsync(p => p.Id == dto.MealPlanId && p.AppUserId == userId);

            var recipeExists = await _context.Recipes
                .AnyAsync(r => r.Id == dto.RecipeId && r.AppUserId == userId);

            if (!planExists || !recipeExists)
            {
                ModelState.AddModelError(string.Empty, "MealPlan або Recipe не знайдено.");
                return await Details(dto.MealPlanId);
            }

            var slot = await _context.MealPlanRecipes
                .FirstOrDefaultAsync(x => x.MealPlanId == dto.MealPlanId
                                       && x.Day == dto.Day
                                       && x.MealType == dto.MealType);

            if (slot is null)
            {
                _context.MealPlanRecipes.Add(new MealPlanRecipe
                {
                    MealPlanId = dto.MealPlanId,
                    RecipeId = dto.RecipeId,
                    Day = dto.Day,
                    MealType = dto.MealType,
                    Notes = dto.Notes
                });
            }
            else
            {
                slot.RecipeId = dto.RecipeId;
                slot.Notes = dto.Notes;
                _context.MealPlanRecipes.Update(slot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = dto.MealPlanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotes(UpdateNotesDto dto)
        {
            var userId = await GetCurrentAppUserIdAsync();

            if (dto.MealPlanRecipeId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Wrong identifier.");
                return await Details(dto.MealPlanId);
            }

            var link = await _context.MealPlanRecipes
                .Include(l => l.MealPlan)
                .FirstOrDefaultAsync(l => l.Id == dto.MealPlanRecipeId
                                       && l.MealPlanId == dto.MealPlanId
                                       && l.MealPlan.AppUserId == userId);

            if (link is null)
            {
                ModelState.AddModelError(string.Empty, "Slot not found.");
                return await Details(dto.MealPlanId);
            }

            link.Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : (dto.Notes!.Length > 300 ? dto.Notes[..300] : dto.Notes);

            _context.MealPlanRecipes.Update(link);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dto.MealPlanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int mealPlanRecipeId, int mealPlanId)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var link = await _context.MealPlanRecipes
                .Include(l => l.MealPlan)
                .FirstOrDefaultAsync(l => l.Id == mealPlanRecipeId
                                       && l.MealPlanId == mealPlanId
                                       && l.MealPlan.AppUserId == userId);

            if (link != null)
            {
                _context.MealPlanRecipes.Remove(link);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = mealPlanId });
        }

        // GET: MealPlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MealPlans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] MealPlan mealPlan)
        {
            if (!ModelState.IsValid)
                return View(mealPlan);

            var userId = await GetCurrentAppUserIdAsync();
            mealPlan.AppUserId = userId;

            _context.Add(mealPlan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: MealPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = await GetCurrentAppUserIdAsync();

            var mealPlan = await _context.MealPlans
                .FirstOrDefaultAsync(p => p.Id == id && p.AppUserId == userId);

            if (mealPlan == null) return NotFound();

            return View(mealPlan);
        }

        // POST: MealPlans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] MealPlan mealPlan)
        {
            if (id != mealPlan.Id) return NotFound();

            var userId = await GetCurrentAppUserIdAsync();

            var existing = await _context.MealPlans
                .FirstOrDefaultAsync(p => p.Id == id && p.AppUserId == userId);

            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
                return View(mealPlan);

            existing.Name = mealPlan.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MealPlans.AnyAsync(e => e.Id == id && e.AppUserId == userId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: MealPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = await GetCurrentAppUserIdAsync();

            var mealPlan = await _context.MealPlans
                .FirstOrDefaultAsync(m => m.Id == id && m.AppUserId == userId);

            if (mealPlan == null) return NotFound();

            return View(mealPlan);
        }

        // POST: MealPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = await GetCurrentAppUserIdAsync();

            var mealPlan = await _context.MealPlans
                .FirstOrDefaultAsync(p => p.Id == id && p.AppUserId == userId);

            if (mealPlan != null)
            {
                _context.MealPlans.Remove(mealPlan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private Task<bool> MealPlanExists(int id, int userId)
        {
            return _context.MealPlans.AnyAsync(e => e.Id == id && e.AppUserId == userId);
        }
    }
}
