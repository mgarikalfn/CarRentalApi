using Application.Common;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Common;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public AcceptBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingDto>> Handle(AcceptBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
                return Result<BookingDto>.Failure("Booking not found.");

            // Approve() checks this.Status internally — no pre-check needed here.
            // If the booking isn't Pending, the aggregate throws DomainException.
            try
            {
                booking.Approve();
            }
            catch (DomainException ex)
            {
                return Result<BookingDto>.Failure(ex.Message);
            }

            await _bookingRepository.UpdateAsync(booking);

            return Result<BookingDto>.Success(new BookingDto
            {
                Id = booking.Id,
                VehicleId = booking.VehicleId,
                RenterId = booking.RenterId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status,
                Subtotal = booking.Price.Subtotal,
                Discount = booking.Price.Discount,
                InsuranceCost = booking.Price.InsuranceCost,
                ServiceFee = booking.Price.ServiceFee,
                TaxRate = booking.Price.TaxRate,
                Total = booking.Price.Total,
                Currency = booking.Price.Currency,
            });
        }
    }
}
