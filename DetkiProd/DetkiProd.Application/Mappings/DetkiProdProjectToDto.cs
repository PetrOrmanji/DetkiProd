using AutoMapper;
using DetkiProd.Application.DTOs;
using DetkiProd.Domain.Entities;

namespace DetkiProd.Application.Mappings;

public class DetkiProdProjectToDto : Profile
{
    public DetkiProdProjectToDto()
    {
        CreateMap<DetkiProdProject, DetkiProdProjectDto>();
    }
}
