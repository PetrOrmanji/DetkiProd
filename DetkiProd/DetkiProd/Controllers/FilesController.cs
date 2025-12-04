using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DetkiProd.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }

    [HttpPost("upload")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Выгрузить файл")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var fileName = await _fileService.UploadAsync(file);
        var videoUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{fileName}";

        return Ok(videoUrl);
    }

    [HttpGet("download/{fileName}")]
    [SwaggerOperation(Summary = "Скачать файл")]
    public IActionResult Download(string fileName)
    {
        var fileStream = _fileService.Get(fileName);
        return File(fileStream, "video/mp4");
    }

    [HttpDelete("delete/{fileName}")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Удалить файл")]
    public IActionResult Delete(string fileName)
    {
        _fileService.Delete(fileName);
        return Ok();
    }

    [HttpGet("getfiles")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Получить список файлов")]
    public IActionResult GetFiles()
    {
        var files = _fileService.GetFiles();
        return Ok(files);
    }
}
