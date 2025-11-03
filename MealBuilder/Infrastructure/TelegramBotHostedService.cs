using System.Text;
using MealBuilder.Infrastructure;
using MealBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public sealed class TelegramBotHostedService : BackgroundService
{
    private readonly ITelegramBotClient _bot;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelegramBotHostedService> _logger;

    public TelegramBotHostedService(
        ITelegramBotClient bot,
        IServiceScopeFactory scopeFactory,
        ILogger<TelegramBotHostedService> logger)
    {
        _bot = bot;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // всі типи
        };

        _bot.StartReceiving(HandleUpdate, HandleError, receiverOptions, stoppingToken);
        _logger.LogInformation("Telegram bot polling started");
        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        try
        {
            var msg = update.Message;
            if (msg?.Text is null) return;

            var text = msg.Text.Trim();

            // Deep-link /start plan_123
            if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
            {
                var payload = text.Split(' ', 2).Length > 1 ? text.Split(' ', 2)[1] : "";
                if (payload.StartsWith("plan_", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(payload.AsSpan("plan_".Length), out var planId))
                {
                    await SendPlanAsync(msg.Chat.Id, planId, ct);
                    return;
                }

                await client.SendTextMessageAsync(msg.Chat.Id,
                    "Вітаю! Надішли команду `/plan <id>` щоб отримати розклад 🍽",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }
            if (text.StartsWith("/plan", StringComparison.OrdinalIgnoreCase))
            {
                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    await client.SendTextMessageAsync(msg.Chat.Id,
                        "Спробуй так: `/plan 5` — де 5 це ID MealPlan.",
                        parseMode: ParseMode.Markdown, cancellationToken: ct);
                    return;
                }

                await SendPlanAsync(msg.Chat.Id, id, ct);
                return;
            }

            await client.SendTextMessageAsync(msg.Chat.Id,
                "Команди: `/plan <id>` або `/start plan_<id>` (deep-link).",
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HandleUpdate error");
        }
    }

    private Task HandleError(ITelegramBotClient client, Exception ex, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    private async Task SendPlanAsync(long chatId, int mealPlanId, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MealBuilderDbContext>();

        var plan = await db.MealPlans
            .Include(p => p.MealPlanRecipes)
                .ThenInclude(mpr => mpr.Recipe)
            .FirstOrDefaultAsync(p => p.Id == mealPlanId, ct);

        if (plan is null)
        {
            await _bot.SendTextMessageAsync(chatId, $"План #{mealPlanId} не знайдено.", cancellationToken: ct);
            return;
        }

        var message = RenderPlan(plan);
        foreach (var chunk in SplitByTelegramLimit(message))
        {
            await _bot.SendTextMessageAsync(chatId, chunk, parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
    private static string RenderPlan(MealBuilder.Models.MealPlan plan)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"*Meal Plan:* _" + Escape(plan.Name) + "_");
        sb.AppendLine();

        var allMealTypes = Enum.GetValues<MealType>().OrderBy(x => x).ToArray();
        var days = Enum.GetValues<DayOfWeek>()
            .OrderBy(d => d == DayOfWeek.Sunday ? -1 : (int)d)
            .ToArray();

        foreach (var day in days)
        {
            sb.AppendLine($"*{day}*");
            foreach (var mt in allMealTypes)
            {
                var slot = plan.MealPlanRecipes
                    .FirstOrDefault(x => x.Day == day && x.MealType == mt);

                var mtEmoji = MealTypeEmoji(mt);
                if (slot?.Recipe is null)
                {
                    sb.AppendLine($"• {mtEmoji} _{mt}_ — `—`");
                }
                else
                {
                    var title = Escape(slot.Recipe.Title);
                    var notes = string.IsNullOrWhiteSpace(slot.Notes) ? "" : $" — _{Escape(slot.Notes!)}_";
                    sb.AppendLine($"• {mtEmoji} _{mt}_ — *{title}*{notes}");
                }
            }
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    private static string MealTypeEmoji(MealType mt) => mt switch
    {
        MealType.Breakfast => "🥣",
        MealType.Lunch => "🥪",
        MealType.Dinner => "🍽",
        MealType.Snack => "🍎",
        MealType.Dessert => "🎂"
    };

    private static string Escape(string s)
        => s.Replace("_", "\\_").Replace("*", "\\*").Replace("`", "\\`");

    private static IEnumerable<string> SplitByTelegramLimit(string text, int limit = 4096)
    {
        if (text.Length <= limit) { yield return text; yield break; }

        var start = 0;
        while (start < text.Length)
        {
            var len = Math.Min(limit, text.Length - start);
            var cut = text.LastIndexOf('\n', start + len - 1, len);
            if (cut <= start) cut = start + len;
            yield return text.Substring(start, cut - start);
            start = cut;
        }
    }
}
