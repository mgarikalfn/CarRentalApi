using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.Booking;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Booking.command
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ActionResult<BookingDto>>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(RentalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActionResult<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);

            if (vehicle == null)
            {
                return new NotFoundObjectResult("Vehicle not found");
            }

            if (vehicle.OwnerId == request.RenterId)
            {
                return new BadRequestObjectResult("You cannot book your own vehicle");
            }

            // Check for date conflicts
            var isAvailable = await _context.Bookings
                .Where(b => b.VehicleId == vehicle.Id &&
                           b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Rejected)
                .AllAsync(b => request.EndDate < b.StartDate ||
                              request.StartDate > b.EndDate,
                          cancellationToken);

            if (!isAvailable)
            {
                return new BadRequestObjectResult("The vehicle is not available for the selected dates");
            }

            // Calculate total price
            var days = (request.EndDate - request.StartDate).Days;
            var totalPrice = days * vehicle.DailyPrice;

            var booking = new CarRentalApi.Entities.Booking
            {
                VehicleId = request.VehicleId,
                RenterId = request.RenterId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);

            // Return the created booking with 201 status
            return new CreatedResult(
                $"/api/bookings/{booking.Id}",
                _mapper.Map<BookingDto>(booking));
        }
    }
}