using Application.Common;
using Application.Dto.Payment;
using MediatR;

namespace Application.Features.Payments.Queries;

public class GetPaymentsByBookingQuery : IRequest<Result<List<PaymentDto>>>
{
    public Guid BookingId { get; set; }
}
