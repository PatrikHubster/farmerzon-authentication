using AutoMapper;
using Microsoft.AspNetCore.Identity;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthenticationManager.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Person
            CreateMap<IdentityUser, DTO.PersonOutput>();
        }
    }
}
