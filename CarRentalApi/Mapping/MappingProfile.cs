using Application.Dto.Availablity;
using Application.Dto.Booking;
using Application.Dto.User;
using Application.Dto.vehicle;
using Application.Features.Availability.Command;
using Application.Features.Vehicle.Command;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace CarRentalApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain → DTO
            CreateMap<ApplicationUser, UserProfileDto>();
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<ApplicationUser, OwnerDto>();
            CreateMap<Availability, AvailabilityDto>();

            // Booking → BookingDto: Booking holds VehicleId/RenterId only (no navigation properties per DDD)
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
                .ForMember(dest => dest.Renter, opt => opt.Ignore());

            // Vehicle → VehicleDto: map from Specification value object
            CreateMap<Domain.Entities.Vehicle, VehicleDto>()
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Specification.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Specification.Model))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.Specification.LicensePlate))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Specification.Color));

            // DTO → Command
            CreateMap<CreateVehicleDto, CreateVehicleCommand>();
            CreateMap<UpdateVehicleDto, UpdateVehicleCommand>();
            CreateMap<CreateAvailabilityCommand, Availability>();

            // VehiclePhoto → VehicleImageDto
            CreateMap<VehiclePhoto, VehicleImageDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Url));
        }
    }
}
