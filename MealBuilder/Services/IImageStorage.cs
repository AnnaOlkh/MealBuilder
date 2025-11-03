namespace MealBuilder.Services;

public interface IImageStorage
{
    Task<string> UploadAsync(IFormFile file, CancellationToken ct = default);
}