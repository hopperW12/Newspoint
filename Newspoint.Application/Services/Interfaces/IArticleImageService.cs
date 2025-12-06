namespace Newspoint.Application.Services.Interfaces;

public interface IArticleImageService
{
    Task<string?> SaveImageAsync(string fileName, string contentType, Stream content);
    Task DeleteImageAsync(string? imagePath);
    Task<string?> ReplaceImageAsync(string? oldPath, string fileName, string contentType, Stream content);
}
