using AutoMapper;
using Application.Dto.Booking;
using Domain.Abstraction;
using FluentResults;
using MediatR;

namespace Application.Features.Booking.Query
{
    public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, Result<List<BookingDto>>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        
        public GetBookingQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<BookingDto>>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(request.UserId);
            
            if (bookings == null || !bookings.Any())
            {
                return Result.Fail("No bookings found");
            }
           
            return Result.Ok(_mapper.Map<List<BookingDto>>(bookings));
        }
    }
}
