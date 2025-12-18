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
        return File(fileStream, "video/mp4", true);
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

    [HttpGet("downloadmain")]
    [SwaggerOperation(Summary = "Скачать главный файл")]
    public IActionResult Download()
    {
        var fileStream = _fileService.GetMain();
        return File(fileStream, "video/mp4");
    }

    [HttpPost("uploadmain")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Выгрузить главный файл")]
    public async Task<IActionResult> UploadMain(IFormFile file)
    {
        var fileName = await _fileService.UploadMainAsync(file);
        var videoUrl = $"{Request.Scheme}://{Request.Host}/api/files/downloadmain";

        return Ok(videoUrl);
    }
}
