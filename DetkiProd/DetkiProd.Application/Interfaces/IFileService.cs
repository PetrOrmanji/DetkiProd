using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file);
    Task<string> UploadAsync(MemoryStream memoryStream, string fileNameWithExt);
    FileStream Get(string fileName);
    string[] GetFiles();
    void Delete(string fileName);
}
