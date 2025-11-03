using System.Security.Claims;
using MealBuilder.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealBuilder.Controllers
{
    public abstract class AppControllerBase : Controller
    {
        protected readonly MealBuilderDbContext _context;

        protected AppControllerBase(MealBuilderDbContext context)
        {
            _context = context;
        }

        protected Task<int> GetCurrentAppUserIdAsync()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                throw new UnauthorizedAccessException("User is not authenticated");

            var provider = "Google";
            var providerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return _context.AppUsers
                .Where(u => u.Provider == provider && u.ProviderUserId == providerUserId)
                .Select(u => u.Id)
                .SingleAsync();
        }
    }
}
