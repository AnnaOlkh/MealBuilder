using MealBuilder.Infrastructure;
using MealBuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Controllers.API
{
    [Route("api/recipes")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    public class RecipesApiController : ControllerBase
    {
        private readonly MealBuilderDbContext _context;
        public RecipesApiController(MealBuilderDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            var items = await _context.Recipes.AsNoTracking().ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            var item = await _context.Recipes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Recipe input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Recipe
            {
                Title = input.Title,
                Description = input.Description,
                Category = input.Category,
                Calories = input.Calories,
                ImageUrl = input.ImageUrl
            };
            _context.Recipes.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { ok = true, id = entity.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Recipe input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _context.Recipes.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Title = input.Title;
            entity.Description = input.Description;
            entity.Category = input.Category;
            entity.Calories = input.Calories;
            entity.ImageUrl = input.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(new { ok = true });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Recipes.FindAsync(id);
            if (entity is null) return NotFound();

            _context.Recipes.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
