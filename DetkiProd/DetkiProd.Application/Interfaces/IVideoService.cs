using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Interfaces;

public interface IVideoService
{
    Task<string> UploadAsync(IFormFile file);
    Stream Get(string fileName);
    string[] GetFiles();
    void Delete(string fileName);
}
