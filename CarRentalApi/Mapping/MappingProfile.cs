using AutoMapper;
using CarRentalApi.Dto;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;

namespace CarRentalApi.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //domain to dto
            CreateMap<ApplicationUser, UserProfileDto>();


            //dto to domain
            CreateMap<CreateVehicleDto, Vehicle>()
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // We'll handle this manually
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true));

            CreateMap<Vehicle, VehicleDto>();
            CreateMap<ApplicationUser, OwnerDto>();
            CreateMap<UpdateVehicleDto, Vehicle>();
                
                
        }
    }
}
