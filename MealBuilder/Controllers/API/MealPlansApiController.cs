using MealBuilder.Infrastructure;
using MealBuilder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Controllers.API
{
    [Route("api/mealplans")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    public class MealPlansApiController : ControllerBase
    {
        private readonly MealBuilderDbContext _context;
        public MealPlansApiController(MealBuilderDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MealPlan>>> GetAll()
        {
            var items = await _context.MealPlans.AsNoTracking().ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MealPlan>> GetById(int id)
        {
            var item = await _context.MealPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MealPlan input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new MealPlan { Name = input.Name };
            _context.MealPlans.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { ok = true, id = entity.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MealPlan input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = await _context.MealPlans.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Name = input.Name;
            await _context.SaveChangesAsync();

            return Ok(new { ok = true });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.MealPlans.FindAsync(id);
            if (entity is null) return NotFound();

            _context.MealPlans.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok(new { ok = true });
        }
    }
}
