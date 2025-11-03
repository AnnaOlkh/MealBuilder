using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace MealBuilder.Services;
public class CloudinaryImageStorage : IImageStorage
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorage(IConfiguration config)
    {
        var cloudName = config["Cloudinary:CloudName"];
        var apiKey = config["Cloudinary:ApiKey"];
        var apiSecret = config["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException("Cloudinary configuration is missing.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadAsync(IFormFile file, CancellationToken ct = default)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "mealbuilder/recipes",
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, ct);

        if (result.Error != null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
        }
        return result.SecureUrl.ToString();
    }
}
