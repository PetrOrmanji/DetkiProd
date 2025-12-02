using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Services;

public class VideoService : IVideoService
{
    private const string VideosDirectory = "Videos";

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (!Directory.Exists(VideosDirectory))
            Directory.CreateDirectory(VideosDirectory);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(VideosDirectory, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    public Stream Get(string fileName)
    {
        var filePath = Path.Combine(VideosDirectory, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public void Delete(string fileName)
    {
        var filePath = Path.Combine(VideosDirectory, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        File.Delete(filePath);
    }

    public string[] GetFiles()
    {
        return Directory.Exists(VideosDirectory) 
            ? Directory.GetFiles(VideosDirectory)
            : Array.Empty<string>();
    }
}
