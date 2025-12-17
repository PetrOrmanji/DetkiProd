using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<string> UploadAsync(FileStream fileStreamSource, string fileNameWithExt);
    FileStream Get(string fileName);
    FileStream GetMain();
    Task<string> UploadMainAsync(IFormFile file);
    Task<string> UploadMainAsync(FileStream fileStreamSource, string fileNameWithExt);
    string[] GetFiles();
    void Delete(string fileName);
    FileStream GetTelegramFile(string filePath);
}
