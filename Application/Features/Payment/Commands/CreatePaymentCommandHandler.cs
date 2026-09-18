using Application.Abstractions;
using Application.Common;
using Application.Dto.Payment;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Payments.Commands;

public class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, Result<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IBookingRepository bookingRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaymentDto>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        // Cross-aggregate check: booking must exist
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking is null)
            return Result<PaymentDto>.Failure("Booking not found.");

        // Only the renter of the booking can pay for it
        if (booking.RenterId != request.PayerId)
            return Result<PaymentDto>.Failure("Only the renter of a booking can pay for it.");

        // Booking must be in Approved state to accept payment
        if (booking.Status != BookingStatus.Approved)
            return Result<PaymentDto>.Failure("Payment can only be made for an approved booking.");

        // Amount reconciliation: must exactly match the booking total
        if (request.Amount != booking.Price.Total)
            return Result<PaymentDto>.Failure(
                $"Payment amount {request.Amount} does not match booking total {booking.Price.Total}.");

        var paidAmount = new Money(request.Amount, booking.Price.Currency);
        var payment = Domain.Entities.Payment.Create(
            request.BookingId,
            request.PayerId,
            paidAmount,
            request.Method);

        // DuplicateActivePaymentException is translated from PostgresException inside the repository.
        // Application never sees PostgresException or DbUpdateException.
        try
        {
            await _paymentRepository.AddAsync(payment, cancellationToken);
        }
        catch (DuplicateActivePaymentException ex)
        {
            return Result<PaymentDto>.Failure(ex.Message);
        }

        return Result<PaymentDto>.Success(_mapper.Map<PaymentDto>(payment));
    }
}
