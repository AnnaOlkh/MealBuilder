namespace MealBuilder.Infrastructure;

using MealBuilder.Models;
using Microsoft.EntityFrameworkCore;

public class MealBuilderDbContext : DbContext
{
    public MealBuilderDbContext(DbContextOptions<MealBuilderDbContext> options) 
        : base(options) { }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<MealPlan> MealPlans { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<MealPlanRecipe> MealPlanRecipes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MealPlanRecipe>()
        .HasIndex(mpr => new {
            mpr.MealPlanId,
            mpr.Day,
            mpr.MealType,
            mpr.RecipeId
        })
        .IsUnique();

        modelBuilder.Entity<MealPlanRecipe>()
        .HasIndex(m => new { m.MealPlanId, m.Day, m.MealType });

        modelBuilder.Entity<MealPlanRecipe>()
            .HasOne(mpr => mpr.MealPlan)
            .WithMany(mp => mp.MealPlanRecipes)
            .HasForeignKey(mpr => mpr.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MealPlanRecipe>()
            .HasOne(mpr => mpr.Recipe)
            .WithMany(r => r.MealPlanRecipes)
            .HasForeignKey(mpr => mpr.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
