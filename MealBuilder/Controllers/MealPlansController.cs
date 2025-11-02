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
using static MealBuilder.ViewModels.RecipeIngredientsVm;

namespace MealBuilder.Controllers
{
    public class MealPlansController : Controller
    {
        private readonly MealBuilderDbContext _context;

        public MealPlansController(MealBuilderDbContext context)
        {
            _context = context;
        }

        // GET: MealPlans
        public async Task<IActionResult> Index()
        {
            return View(await _context.MealPlans.ToListAsync());
        }

        // GET: MealPlans/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.MealPlans
                .Include(p => p.MealPlanRecipes)
                    .ThenInclude(mpr => mpr.Recipe)
                .FirstOrDefaultAsync(p => p.Id == id);

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
                        Notes = existing?.Notes
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
                RecipeOptions = await _context.Recipes
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
            if (!ModelState.IsValid)
                return await Details(dto.MealPlanId);

            var planExists = await _context.MealPlans.AnyAsync(p => p.Id == dto.MealPlanId);
            var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == dto.RecipeId);
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
            // базова перевірка
            if (dto.MealPlanRecipeId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Wrong identifier.");
                return await Details(dto.MealPlanId);
            }

            var link = await _context.MealPlanRecipes.FindAsync(dto.MealPlanRecipeId);
            if (link is null)
            {
                ModelState.AddModelError(string.Empty, "Slot not found.");
                return await Details(dto.MealPlanId);
            }

            link.Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : (dto.Notes!.Length > 300 ? dto.Notes.Substring(0, 300) : dto.Notes);

            _context.MealPlanRecipes.Update(link);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = dto.MealPlanId });
        }
        // POST: /MealPlans/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int mealPlanRecipeId, int mealPlanId)
        {
            var link = await _context.MealPlanRecipes.FindAsync(mealPlanRecipeId);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] MealPlan mealPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mealPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mealPlan);
        }

        // GET: MealPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealPlan = await _context.MealPlans.FindAsync(id);
            if (mealPlan == null)
            {
                return NotFound();
            }
            return View(mealPlan);
        }

        // POST: MealPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] MealPlan mealPlan)
        {
            if (id != mealPlan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealPlanExists(mealPlan.Id))
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
            return View(mealPlan);
        }

        // GET: MealPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealPlan = await _context.MealPlans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            return View(mealPlan);
        }

        // POST: MealPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mealPlan = await _context.MealPlans.FindAsync(id);
            if (mealPlan != null)
            {
                _context.MealPlans.Remove(mealPlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealPlanExists(int id)
        {
            return _context.MealPlans.Any(e => e.Id == id);
        }
    }
}
