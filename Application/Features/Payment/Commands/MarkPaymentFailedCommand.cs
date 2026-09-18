using Application.Common;
using Application.Dto.Payment;
using MediatR;

namespace Application.Features.Payments.Commands;

public class MarkPaymentFailedCommand : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
