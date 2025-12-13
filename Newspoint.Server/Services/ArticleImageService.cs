using Newspoint.Application.Services.Interfaces;

namespace Newspoint.Server.Services;

public class ArticleImageService : IArticleImageService
{
    private const long MaxFileSize = 2 * 1024 * 1024; // 2MB
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png"];
    
    private readonly IWebHostEnvironment _environment;

    public ArticleImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> SaveImage(string fileName, string contentType, Stream content)
    {
        if (content == null || !content.CanRead)
            return null;

        if (!AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException("Nepovolený typ souboru.");

        if (content.Length > MaxFileSize)
            throw new InvalidOperationException("Soubor je příliš velký.");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "articles");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Vytvoření unikátního názvu souboru se stejnou příponou.
        var extension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        content.Position = 0;
        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fileStream);
        }

        // Vrací relativní cestu použitelnou na frontendu.
        return $"/images/articles/{uniqueFileName}";
    }

    public Task DeleteImage(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return Task.CompletedTask;

        // Převedení relativní URL cesty na fyzickou cestu k souboru.
        var relativePath = imagePath.TrimStart('/')
            .Replace("/", Path.DirectorySeparatorChar.ToString());
        var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public async Task<string?> ReplaceImage(string? oldPath, string fileName, string contentType, Stream content)
    {
        await DeleteImage(oldPath);
        return await SaveImage(fileName, contentType, content);
    }
}
