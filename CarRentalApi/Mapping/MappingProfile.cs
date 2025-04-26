using AutoMapper;
using CarRentalApi.Application.Availability.Command;
using CarRentalApi.Application.Vehicle.command;
using CarRentalApi.Dto;
using CarRentalApi.Dto.Availablity;
using CarRentalApi.Dto.Booking;
using CarRentalApi.Dto.User;
using CarRentalApi.Dto.vehicle;
using Domain.Entities;

namespace CarRentalApi.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //domain to dto
            CreateMap<ApplicationUser, UserProfileDto>();
            CreateMap<Availability, AvailabilityDto>();
            CreateMap<Booking, BookingDto>()
     .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
     .ForMember(dest => dest.Renter, opt => opt.MapFrom(src => src.Renter));
            //dto to domain
            CreateMap<CreateVehicleCommand, Vehicle>()
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // We'll handle this manually
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(_ => true));

            CreateMap<CreateVehicleDto, CreateVehicleCommand>();
            CreateMap<UpdateVehicleDto, UpdateVehicleCommand>();

            CreateMap<Vehicle, VehicleDto>();
            CreateMap<ApplicationUser, OwnerDto>();
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UpdateVehicleCommand, Vehicle>();

            CreateMap<CreateAvailabilityCommand, Availability>();

            CreateMap<VehicleImage, VehicleImageDto>();

            CreateMap<CreateBookingDto,Booking>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => BookingStatus.Pending));

            // Reverse mapping if needed
            CreateMap<VehicleImageDto, VehicleImage>();


        }
    }
}
