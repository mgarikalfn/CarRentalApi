using Application.Common;
using Application.Dto.Payment;
using MediatR;

namespace Application.Features.Payments.Commands;

public class MarkPaymentSucceededCommand : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
}
