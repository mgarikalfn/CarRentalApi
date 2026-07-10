using AutoMapper;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(IBookingRepository bookingRepository, IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);

            if (vehicle == null)
            {
                return Result.Fail("Vehicle not found");
            }

            if (vehicle.OwnerId == request.RenterId)
            {
                return Result.Fail("You cannot book your own vehicle");
            }

            // Check for date conflicts
            var isBooked = await _bookingRepository.IsVehicleBookedAsync(vehicle.Id, request.StartDate, request.EndDate);

            if (isBooked)
            {
                return Result.Fail("The vehicle is not available for the selected dates");
            }

            // Calculate total price
            var days = (request.EndDate - request.StartDate).Days;
            var totalPrice = days * vehicle.DailyPrice;

            var booking = new Domain.Entities.Booking
            {
                VehicleId = request.VehicleId,
                RenterId = request.RenterId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);

            return Result.Ok(_mapper.Map<BookingDto>(booking));
        }
    }
}