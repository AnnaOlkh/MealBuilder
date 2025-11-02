using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealBuilder.Infrastructure;
using MealBuilder.Models;

namespace MealBuilder.Controllers.Api;

[ApiController]
[Route("api/mealplans/{mealPlanId:int}/slots")]
[Produces("application/json", "application/xml")]
public class MealPlanSlotsApiController : ControllerBase
{
    private readonly MealBuilderDbContext _context;
    public MealPlanSlotsApiController(MealBuilderDbContext context) => _context = context;

    // GET /api/mealplans/{mealPlanId}/slots
    [HttpGet]
    public async Task<IActionResult> GetSlots(int mealPlanId)
    {
        var plan = await _context.MealPlans
            .Include(p => p.MealPlanRecipes)
                .ThenInclude(mpr => mpr.Recipe)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == mealPlanId);

        if (plan is null) return NotFound();

        var slots = plan.MealPlanRecipes
            .Select(s => new {
                MealPlanRecipeId = s.Id,
                s.MealPlanId,
                s.Day,
                s.MealType,
                s.RecipeId,
                RecipeTitle = s.Recipe?.Title,
                s.Notes
            })
            .OrderBy(x => x.Day).ThenBy(x => x.MealType)
            .ToList();

        return Ok(new { plan.Id, plan.Name, slots });
    }

    // PUT /api/mealplans/{mealPlanId}/slots
    [HttpPut]
    public async Task<IActionResult> Upsert(int mealPlanId, [FromBody] MealPlanRecipe body)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var planOk = await _context.MealPlans.AnyAsync(p => p.Id == mealPlanId);
        var recipeOk = await _context.Recipes.AnyAsync(r => r.Id == body.RecipeId);
        if (!planOk || !recipeOk) return NotFound();

        var slot = await _context.MealPlanRecipes
            .FirstOrDefaultAsync(x => x.MealPlanId == mealPlanId && x.Day == body.Day && x.MealType == body.MealType);

        if (slot is null)
        {
            slot = new MealPlanRecipe
            {
                MealPlanId = mealPlanId,
                RecipeId = body.RecipeId,
                Day = body.Day,
                MealType = body.MealType,
                Notes = string.IsNullOrWhiteSpace(body.Notes) ? null :
                        (body.Notes!.Length > 300 ? body.Notes.Substring(0, 300) : body.Notes)
            };
            _context.MealPlanRecipes.Add(slot);
        }
        else
        {
            slot.RecipeId = body.RecipeId;
            slot.Notes = string.IsNullOrWhiteSpace(body.Notes) ? null :
                         (body.Notes!.Length > 300 ? body.Notes.Substring(0, 300) : body.Notes);
        }

        await _context.SaveChangesAsync();
        return Ok(new { ok = true, id = slot.Id });
    }

    // PATCH /api/mealplans/{mealPlanId}/slots/{slotId}/notes
    public class PatchNotes { public string? Notes { get; set; } }

    [HttpPatch("{slotId:int}/notes")]
    public async Task<IActionResult> PatchNotesAction(int mealPlanId, int slotId, [FromBody] PatchNotes body)
    {
        var slot = await _context.MealPlanRecipes
            .FirstOrDefaultAsync(x => x.Id == slotId && x.MealPlanId == mealPlanId);

        if (slot is null) return NotFound();

        slot.Notes = string.IsNullOrWhiteSpace(body.Notes) ? null :
                     (body.Notes!.Length > 300 ? body.Notes.Substring(0, 300) : body.Notes);

        await _context.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    // DELETE /api/mealplans/{mealPlanId}/slots/{slotId}
    [HttpDelete("{slotId:int}")]
    public async Task<IActionResult> DeleteSlot(int mealPlanId, int slotId)
    {
        var slot = await _context.MealPlanRecipes
            .FirstOrDefaultAsync(x => x.Id == slotId && x.MealPlanId == mealPlanId);

        if (slot is null) return NotFound();

        _context.MealPlanRecipes.Remove(slot);
        await _context.SaveChangesAsync();
        return Ok(new { ok = true });
    }
}
