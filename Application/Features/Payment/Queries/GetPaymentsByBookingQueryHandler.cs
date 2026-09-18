using Application.Abstractions;
using Application.Common;
using Application.Dto.Payment;
using AutoMapper;
using MediatR;

namespace Application.Features.Payment.Queries;

public class GetPaymentsByBookingQueryHandler
    : IRequestHandler<GetPaymentsByBookingQuery, Result<List<PaymentDto>>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByBookingQueryHandler(
        IPaymentRepository paymentRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PaymentDto>>> Handle(
        GetPaymentsByBookingQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        var dtos = _mapper.Map<List<PaymentDto>>(payments);
        return Result<List<PaymentDto>>.Success(dtos);
    }
}
