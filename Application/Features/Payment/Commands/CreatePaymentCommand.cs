using Application.Common;
using Application.Dto.Payment;
using Domain.Enums;
using MediatR;

namespace Application.Features.Payment.Commands;

public class CreatePaymentCommand : IRequest<Result<PaymentDto>>
{
    public Guid BookingId { get; set; }
    public Guid PayerId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
}
