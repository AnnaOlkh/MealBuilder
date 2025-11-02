using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealBuilder.Infrastructure;
using MealBuilder.Models;

namespace MealBuilder.Controllers.API;

[ApiController]
[Route("api/recipes/{recipeId:int}/ingredients")]
[Produces("application/json", "application/xml")]
public class RecipeIngredientsApiController : ControllerBase
{
    private readonly MealBuilderDbContext _context;
    public RecipeIngredientsApiController(MealBuilderDbContext context) => _context = context;

    // GET /api/recipes/{recipeId}/ingredients
    [HttpGet]
    public async Task<IActionResult> List(int recipeId)
    {
        var exists = await _context.Recipes.AnyAsync(r => r.Id == recipeId);
        if (!exists) return NotFound();

        var rows = await _context.RecipeIngredients
            .AsNoTracking()
            .Where(ri => ri.RecipeId == recipeId)
            .Select(ri => new {
                ri.RecipeId,
                ri.IngredientId,
                IngredientName = ri.Ingredient.Name,
                ri.Quantity,
                ri.Unit
            })
            .ToListAsync();

        return Ok(rows);
    }

    // PUT /api/recipes/{recipeId}/ingredients/{ingredientId}
    // upsert композитного зв'язку + кількість/одиниці
    [HttpPut("{ingredientId:int}")]
    public async Task<IActionResult> Upsert(int recipeId, int ingredientId, [FromBody] RecipeIngredient body)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var recipeOk = await _context.Recipes.AnyAsync(r => r.Id == recipeId);
        var ingrOk = await _context.Ingredients.AnyAsync(i => i.Id == ingredientId);
        if (!recipeOk || !ingrOk) return NotFound();

        var link = await _context.RecipeIngredients.FindAsync(recipeId, ingredientId);
        if (link is null)
        {
            link = new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Quantity = body.Quantity,
                Unit = body.Unit
            };
            _context.RecipeIngredients.Add(link);
        }
        else
        {
            link.Quantity = body.Quantity;
            link.Unit = body.Unit;
        }

        await _context.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    // DELETE /api/recipes/{recipeId}/ingredients/{ingredientId}
    [HttpDelete("{ingredientId:int}")]
    public async Task<IActionResult> Remove(int recipeId, int ingredientId)
    {
        var link = await _context.RecipeIngredients.FindAsync(recipeId, ingredientId);
        if (link is null) return NotFound();

        _context.RecipeIngredients.Remove(link);
        await _context.SaveChangesAsync();
        return Ok(new { ok = true });
    }
}
