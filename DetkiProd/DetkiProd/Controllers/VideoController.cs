using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DetkiProd.Controllers;

[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly IVideoService _videoService;

    public VideoController(IVideoService videoService)
    {
        _videoService = videoService ?? throw new ArgumentNullException(nameof(videoService));
    }

    [HttpPost("upload")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Выгрузить файл")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var fileName = await _videoService.UploadAsync(file);
        var videoUrl = $"{Request.Scheme}://{Request.Host}/api/videos/download/{fileName}";

        return Ok(videoUrl);
    }

    [HttpGet("download/{fileName}")]
    [SwaggerOperation(Summary = "Скачать файл")]
    public IActionResult Download(string fileName)
    {
        var fileStream = _videoService.Get(fileName);
        return File(fileStream, "video/mp4");
    }

    [HttpDelete("delete/{fileName}")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Удалить файл")]
    public IActionResult Delete(string fileName)
    {
        _videoService.Delete(fileName);
        return Ok();
    }

    [HttpGet("getfiles")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Получить список файлов")]
    public IActionResult GetFiles()
    {
        var files = _videoService.GetFiles();
        return Ok(files);
    }
}
