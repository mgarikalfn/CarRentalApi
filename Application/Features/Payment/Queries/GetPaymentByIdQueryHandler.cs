using Application.Abstractions;
using Application.Common;
using Application.Dto.Payment;
using Application.Features.Payments.Queries;
using AutoMapper;
using MediatR;

namespace Application.Features.Payments.Queries;

public class GetPaymentByIdQueryHandler
    : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(
        IPaymentRepository paymentRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaymentDto>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Failure("Payment not found.");

        return Result<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
    }
}
