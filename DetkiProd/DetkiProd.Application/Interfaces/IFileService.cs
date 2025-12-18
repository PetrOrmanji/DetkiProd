using Microsoft.AspNetCore.Http;

namespace DetkiProd.Application.Interfaces;

public interface IFileService
{
    FileStream GetMainVideo();
    FileStream GetProjectVideo(string fileName);
    FileStream GetFileByPath(string filePath);
    string[] GetVideos();
    Task<string> UploadMainVideoAsync(IFormFile file);
    Task<string> UploadMainVideoAsync(FileStream fileStreamSource, string fileNameWithExt);
    Task<string> UploadProjectVideoAsync(IFormFile file);
    Task<string> UploadProjectVideoAsync(FileStream fileStreamSource, string fileNameWithExt);
    void DeleteVideo(string fileName);
}
