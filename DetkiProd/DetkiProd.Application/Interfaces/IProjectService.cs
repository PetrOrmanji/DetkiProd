using DetkiProd.Application.DTOs;

namespace DetkiProd.Application.Interfaces;

public interface IProjectService
{
    Task<List<DetkiProdProjectDto>> GetProjectsAsync();
    Task<DetkiProdProjectDto> GetProjectByIdAsync(Guid id);
    Task<DetkiProdProjectDto> AddProjectAsync(string name, string tools, string year, string videoUrl);
    Task DeleteProjectByIdAsync(Guid id);
}
