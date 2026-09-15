using AutoMapper;
using Application.Common;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
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
                return Result<BookingDto>.Failure("Vehicle not found");
            }

            if (vehicle.OwnerId == request.RenterId)
            {
                return Result<BookingDto>.Failure("You cannot book your own vehicle");
            }

            // Check for date conflicts
            var isBooked = await _bookingRepository.IsVehicleBookedAsync(vehicle.Id, request.StartDate, request.EndDate);

            if (isBooked)
            {
                return Result<BookingDto>.Failure("The vehicle is not available for the selected dates");
            }

            // Calculate total price using BookingPrice value object
            var days = (request.EndDate - request.StartDate).Days;
            var dailyPrice = vehicle.Price.DailyPrice;
            var totalPrice = days * dailyPrice;
            var bookingPrice = new BookingPrice(totalPrice, 0, 0, 0, 0, vehicle.Price.Currency);

            var booking = Domain.Entities.Booking.Create(
                vehicle.Id,
                request.RenterId,
                request.StartDate,
                request.EndDate,
                request.PickUpLocation,
                request.DropOffLocation,
                bookingPrice);

            await _bookingRepository.AddAsync(booking);

            return Result<BookingDto>.Success(_mapper.Map<BookingDto>(booking));
        }
    }
}