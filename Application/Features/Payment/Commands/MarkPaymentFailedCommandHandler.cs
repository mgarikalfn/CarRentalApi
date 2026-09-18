using Application.Abstractions;
using Application.Common;
using Application.Dto.Payment;
using AutoMapper;
using Domain.Common;
using MediatR;

namespace Application.Features.Payment.Commands;

public class MarkPaymentFailedCommandHandler
    : IRequestHandler<MarkPaymentFailedCommand, Result<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public MarkPaymentFailedCommandHandler(
        IPaymentRepository paymentRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaymentDto>> Handle(
        MarkPaymentFailedCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Failure("Payment not found.");

        try
        {
            payment.MarkFailed(request.Reason);
        }
        catch (DomainException ex)
        {
            return Result<PaymentDto>.Failure(ex.Message);
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        return Result<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
    }
}
