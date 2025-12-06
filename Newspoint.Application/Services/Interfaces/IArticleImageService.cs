namespace Newspoint.Application.Services.Interfaces;

public interface IArticleImageService
{
    Task<string?> SaveImage(string fileName, string contentType, Stream content);
    Task DeleteImage(string? imagePath);
    Task<string?> ReplaceImage(string? oldPath, string fileName, string contentType, Stream content);
}
