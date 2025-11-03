using MealBuilder.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MealBuilder.Seeding;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Telegram.Bot;
using MealBuilder.Models;
using System.Security.Claims;
using MealBuilder.Services;

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
builder.Services.AddSingleton<IImageStorage, CloudinaryImageStorage>();
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
{
    var token = builder.Configuration["Telegram:BotToken"]
               ?? throw new InvalidOperationException("Telegram bot token missing");
    return new TelegramBotClient(token);
});
builder.Services.AddHostedService<TelegramBotHostedService>();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.Events.OnCreatingTicket = async context =>
        {
            using var scope = context.HttpContext.RequestServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<MealBuilderDbContext>();

            var provider = "Google";
            var providerUserId = context.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var email = context.Principal!.FindFirstValue(ClaimTypes.Email)!;
            var name = context.Principal!.FindFirstValue(ClaimTypes.Name);

            var user = await db.AppUsers
                .FirstOrDefaultAsync(u => u.Provider == provider && u.ProviderUserId == providerUserId);

            if (user is null)
            {
                user = await db.AppUsers
                    .FirstOrDefaultAsync(u => u.Provider == provider && u.Email == email);
            }
            if (user is null)
            {
                user = new AppUser
                {
                    Provider = provider,
                    ProviderUserId = providerUserId,
                    Email = email,
                    Name = name
                };
                db.AppUsers.Add(user);
            }
            else
            {
                user.ProviderUserId = providerUserId;
                user.Email = email;
                user.Name = name;
            }

            await db.SaveChangesAsync();
        };
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
