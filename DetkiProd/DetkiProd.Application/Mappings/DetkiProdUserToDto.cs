using AutoMapper;
using DetkiProd.Application.DTOs;
using DetkiProd.Domain.Entities;

namespace DetkiProd.Application.Mappings;

public class DetkiProdUserToDto : Profile
{
    public DetkiProdUserToDto()
    {
        CreateMap<DetkiProdUser, DetkiProdUserDto>();
    }
}
