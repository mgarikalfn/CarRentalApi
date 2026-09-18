using Application.Common;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Common;
using MediatR;

namespace Application.Features.Bookings.Command
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public RejectBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingDto>> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
                return Result<BookingDto>.Failure("Booking not found.");

            // Reject() checks this.Status internally — aggregate enforces Pending-only guard.
            try
            {
                booking.Reject(string.IsNullOrWhiteSpace(request.Reason)
                    ? "No reason provided"
                    : request.Reason);
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
                RejectionReason = booking.RejectionReason,
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
