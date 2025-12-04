using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Services;

public class FileService : IFileService
{
    private const string VideosDirectory = "Videos";

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (!Directory.Exists(VideosDirectory))
            Directory.CreateDirectory(VideosDirectory);

        var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);
        var fileName = originalFileName + extension;
        var filePath = Path.Combine(VideosDirectory, fileName);

        int counter = 1;
        while (File.Exists(filePath))
        {
            fileName = $"{originalFileName}_{counter}{extension}";
            filePath = Path.Combine(VideosDirectory, fileName);
            counter++;
        }

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }
    public async Task<string> UploadAsync(MemoryStream memoryStream, string fileNameWithExt)
    {
        if (!Directory.Exists(VideosDirectory))
            Directory.CreateDirectory(VideosDirectory);

        var originalFileName = Path.GetFileNameWithoutExtension(fileNameWithExt);
        var extension = Path.GetExtension(fileNameWithExt);
        var fileName = originalFileName + extension;
        var filePath = Path.Combine(VideosDirectory, fileName);

        int counter = 1;
        while (File.Exists(filePath))
        {
            fileName = $"{originalFileName}_{counter}{extension}";
            filePath = Path.Combine(VideosDirectory, fileName);
            counter++;
        }

        memoryStream.Position = 0;
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await memoryStream.CopyToAsync(fileStream);
        return fileName;
    }

    public FileStream Get(string fileName)
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
                       .Select(Path.GetFileName)
                       .Where(name => name != null)
                       .Select(name => name!)
                       .ToArray()
            : Array.Empty<string>();
    }
}
