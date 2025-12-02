using AutoMapper;
using DetkiProd.Application.DTOs;
using DetkiProd.Application.Interfaces;
using DetkiProd.Domain.Entities;
using DetkiProd.Domain.Exceptions;
using DetkiProd.Domain.Repositories;

namespace DetkiProd.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IDetkiProdProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public ProjectService(IDetkiProdProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<DetkiProdProjectDto> AddProjectAsync(string name, string tools, string year, string videoUrl)
    {
        var newProject = DetkiProdProject.Create(name, tools, year, videoUrl);
        await _projectRepository.AddAsync(newProject);

        return _mapper.Map<DetkiProdProjectDto>(newProject);
    }

    public async Task DeleteProjectByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            throw new ProjectNotFoundException(id);
        }

        await _projectRepository.DeleteByIdAsync(id);
    }

    public async Task<DetkiProdProjectDto> GetProjectByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            throw new ProjectNotFoundException(id);
        }

        return _mapper.Map<DetkiProdProjectDto>(project);
    }

    public async Task<List<DetkiProdProjectDto>> GetProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return _mapper.Map<List<DetkiProdProjectDto>>(projects);
    }
}
