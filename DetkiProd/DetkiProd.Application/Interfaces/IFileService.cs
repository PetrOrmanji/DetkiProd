using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<string> UploadAsync(MemoryStream memoryStream, string fileNameWithExt);
    FileStream Get(string fileName);
    FileStream GetMain();
    Task<string> UploadMainAsync(IFormFile file);
    Task<string> UploadMainAsync(MemoryStream memoryStream, string fileNameWithExt);
    string[] GetFiles();
    void Delete(string fileName);
}
