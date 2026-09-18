using Application.Common;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Common;
using MediatR;

namespace Application.Features.Bookings.Command
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public CancelBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingDto>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
                return Result<BookingDto>.Failure("Booking not found.");

            if (booking.RenterId != request.UserId)
                return Result<BookingDto>.Failure("Unauthorized to cancel this booking.");

            // Cancel() checks this.Status internally using the correct && guard:
            //   Status != Pending && Status != Approved → throw
            // No need to duplicate that logic here.
            try
            {
                booking.Cancel(string.IsNullOrWhiteSpace(request.Reason)
                    ? "Cancelled by renter"
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
                CancellationReason = booking.CancellationReason,
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
