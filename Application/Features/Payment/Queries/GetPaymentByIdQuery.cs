using Application.Common;
using Application.Dto.Payment;
using MediatR;

namespace Application.Features.Payments.Queries;

public class GetPaymentByIdQuery : IRequest<Result<PaymentDto>>
{
    public Guid PaymentId { get; set; }
}
