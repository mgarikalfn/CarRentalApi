using Application.Common;
using Application.Dto.Payment;
using MediatR;

namespace Application.Features.Payment.Commands;

public class RefundPaymentCommand : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
