using AutoMapper;
using CarRentalApi.Application.Availability.Command;
using CarRentalApi.Application.Vehicle.command;
using CarRentalApi.Dto;
using CarRentalApi.Dto.Availablity;
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
            CreateMap<Availability, AvailabilityDto>();

            //dto to domain
            CreateMap<CreateVehicleCommand, Vehicle>()
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // We'll handle this manually
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true));

            CreateMap<CreateVehicleDto, CreateVehicleCommand>();
            CreateMap<UpdateVehicleDto, UpdateVehicleCommand>();

            CreateMap<Vehicle, VehicleDto>();
            CreateMap<ApplicationUser, OwnerDto>();
            CreateMap<UpdateVehicleCommand, Vehicle>();

            CreateMap<CreateAvailabilityCommand, Availability>();

            CreateMap<VehicleImage, VehicleImageDto>();

            // Reverse mapping if needed
            CreateMap<VehicleImageDto, VehicleImage>();


        }
    }
}
