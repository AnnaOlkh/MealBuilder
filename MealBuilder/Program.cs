using MealBuilder.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MealBuilder.Seeding;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MealBuilderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MealBuilderDbContext")
    ?? throw new InvalidOperationException("Connection string 'MealBuilderDbContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews()
  .AddXmlSerializerFormatters()
  .AddJsonOptions(o =>
  {
      o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
  });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "MealBuilder API", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MealBuilderDbContext>();
    await SeedData.EnsureSeededAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "MealBuilder API v1");
    o.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
