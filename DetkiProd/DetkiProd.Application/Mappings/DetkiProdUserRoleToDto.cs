using AutoMapper;
using DetkiProd.Application.DTOs;
using DetkiProd.Domain.Entities;

namespace DetkiProd.Application.Mappings;

public class DetkiProdUserRoleToDto : Profile
{
    public DetkiProdUserRoleToDto()
    {
        CreateMap<DetkiProdUserRole, DetkiProdUserRoleDto>();
    }
}
