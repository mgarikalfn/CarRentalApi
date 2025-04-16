using AutoMapper;
using CarRentalApi.Dto;
using CarRentalApi.Entities;

namespace CarRentalApi.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //domain to dto
            CreateMap<ApplicationUser, UserProfileDto>();

        }
    }
}
