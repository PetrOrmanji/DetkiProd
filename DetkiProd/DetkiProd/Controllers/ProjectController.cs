using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DetkiProd.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
    }

    [HttpGet("getprojects")]
    [SwaggerOperation(Summary = "Получить проекты")]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _projectService.GetProjectsAsync();
        return Ok(projects);
    }

    [HttpPost("addproject")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Добавить проект")]
    public async Task<IActionResult> AddProject([FromForm] AddProjectRequest request)
    {
        var project = await _projectService.AddProjectAsync(
            request.Name,
            request.Tools, 
            request.Year,
            request.VideoUrl);

        return Ok(project);
    }

    [HttpDelete("deleteproject/{projectId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Удалить проект")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        var project = await _projectService.GetProjectByIdAsync(projectId);
        await _projectService.DeleteProjectByIdAsync(projectId);

        return Ok();
    }
}

[SwaggerSchema("Запрос для добавления проекта")]
public record AddProjectRequest(string Name, string Tools, string Year, string VideoUrl);
