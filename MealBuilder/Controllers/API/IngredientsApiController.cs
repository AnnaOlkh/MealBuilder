using MealBuilder.Infrastructure;
using MealBuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Controllers.API
{
    [Route("api/ingredients")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    public class IngredientsApiController : ControllerBase
    {
        private readonly MealBuilderDbContext _context;
        public IngredientsApiController(MealBuilderDbContext db) => _context = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            var items = await _context.Ingredients.AsNoTracking().ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            var item = await _context.Ingredients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Ingredient input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Ingredient { Name = input.Name };
            _context.Ingredients.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { ok = true, id = entity.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ingredient input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _context.Ingredients.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Name = input.Name;
            await _context.SaveChangesAsync();

            return Ok(new { ok = true });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Ingredients.FindAsync(id);
            if (entity is null) return NotFound();

            _context.Ingredients.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
