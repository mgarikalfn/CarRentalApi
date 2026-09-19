using Application.Dto.Availablity;
using Application.Dto.DamageReport;
using Application.Dto.Booking;
using Application.Dto.Payment;
using Application.Dto.Review;
using Application.Dto.User;
using Application.Dto.vehicle;
using Application.Features.Availabilities.Command;
using Application.Features.Vehicles.Command;
using AutoMapper;
using Domain.Entities;

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

            // Booking → BookingDto:
            // - VehicleId/RenterId are plain Guid references (no navigation properties per DDD)
            // - Price fields are flattened from BookingPrice value object
            // - Vehicle/Renter left null — handlers populate if needed
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.Subtotal,      opt => opt.MapFrom(src => src.Price.Subtotal))
                .ForMember(dest => dest.Discount,      opt => opt.MapFrom(src => src.Price.Discount))
                .ForMember(dest => dest.InsuranceCost, opt => opt.MapFrom(src => src.Price.InsuranceCost))
                .ForMember(dest => dest.ServiceFee,    opt => opt.MapFrom(src => src.Price.ServiceFee))
                .ForMember(dest => dest.TaxRate,       opt => opt.MapFrom(src => src.Price.TaxRate))
                .ForMember(dest => dest.Total,         opt => opt.MapFrom(src => src.Price.Total))
                .ForMember(dest => dest.Currency,      opt => opt.MapFrom(src => src.Price.Currency))
                .ForMember(dest => dest.Vehicle,       opt => opt.Ignore())
                .ForMember(dest => dest.Renter,        opt => opt.Ignore());

            // Vehicle → VehicleDto: map from Specification value object
            CreateMap<Domain.Entities.Vehicle, VehicleDto>()
                .ForMember(dest => dest.Make,         opt => opt.MapFrom(src => src.Specification.Brand))
                .ForMember(dest => dest.Model,        opt => opt.MapFrom(src => src.Specification.Model))
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.Specification.LicensePlate))
                .ForMember(dest => dest.Color,        opt => opt.MapFrom(src => src.Specification.Color));

            // DTO → Command
            CreateMap<CreateVehicleDto, CreateVehicleCommand>();
            CreateMap<UpdateVehicleDto, UpdateVehicleCommand>();
            CreateMap<CreateAvailabilityCommand, Availability>();

            // VehiclePhoto → VehicleImageDto
            CreateMap<VehiclePhoto, VehicleImageDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Url));

            // Payment → PaymentDto: flatten PaidAmount value object
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Amount,   opt => opt.MapFrom(src => src.PaidAmount.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.PaidAmount.Currency));

            // Review → ReviewDto: all property names match directly
            CreateMap<Review, ReviewDto>();
        }
    }
}
