using Application.Common;
using Application.Dto.Payment;
using MediatR;

namespace Application.Features.Payment.Commands;

public class MarkPaymentSucceededCommand : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
}
