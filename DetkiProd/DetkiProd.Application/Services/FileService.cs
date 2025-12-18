using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Services;

public class FileService : IFileService
{
    private const string VideosDirectory = "Videos";
    private const string MainVideoDirectory = "MainVideo";
    private const string MainVideoFileName = "MainVideo";

    public FileStream GetMainVideo()
    {
        var directoryPath = Path.Combine(VideosDirectory, MainVideoDirectory);
        var mainVideoFileName = Directory.GetFiles(directoryPath).Select(Path.GetFileName).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(mainVideoFileName))
            throw new FileNotFoundException();

        var filePath = Path.Combine(directoryPath, mainVideoFileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public FileStream GetProjectVideo(string fileName)
    {
        var filePath = Path.Combine(VideosDirectory, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public FileStream GetFileByPath(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public string[] GetVideos()
    {
        return Directory.Exists(VideosDirectory)
            ? Directory.GetFiles(VideosDirectory)
                       .Select(Path.GetFileName)
                       .Where(name => name != null)
                       .Select(name => name!)
                       .ToArray()
            : Array.Empty<string>();
    }

    public async Task<string> UploadMainVideoAsync(IFormFile file)
    {
        var directoryPath = Path.Combine(VideosDirectory, MainVideoDirectory);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        else
        {
            Directory.Delete(directoryPath, true);
            Directory.CreateDirectory(directoryPath);
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = MainVideoFileName + extension;
        var filePath = Path.Combine(directoryPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }

    public async Task<string> UploadMainVideoAsync(FileStream fileStreamSource, string fileNameWithExt)
    {
        var directoryPath = Path.Combine(VideosDirectory, MainVideoDirectory);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        else
        {
            Directory.Delete(directoryPath, true);
            Directory.CreateDirectory(directoryPath);
        }

        var extension = Path.GetExtension(fileNameWithExt);
        var fileName = MainVideoFileName + extension;
        var filePath = Path.Combine(directoryPath, fileName);

        fileStreamSource.Position = 0;
        using var fileStreamDestination = new FileStream(filePath, FileMode.Create);
        await fileStreamSource.CopyToAsync(fileStreamDestination);
        return fileName;
    }

    public async Task<string> UploadProjectVideoAsync(IFormFile file)
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
    public async Task<string> UploadProjectVideoAsync(FileStream fileStreamSource, string fileNameWithExt)
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
        
        fileStreamSource.Position = 0;
        using var fileStreamDestination = new FileStream(filePath, FileMode.Create);
        await fileStreamSource.CopyToAsync(fileStreamDestination);
        return fileName;
    }

    public void DeleteVideo(string fileName)
    {
        var filePath = Path.Combine(VideosDirectory, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        File.Delete(filePath);
    }
}
