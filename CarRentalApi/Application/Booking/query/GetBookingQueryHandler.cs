using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.Booking;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Booking.query
{
    public class GetBookingQueryHandler:IRequestHandler<GetBookingQuery, ActionResult<List<BookingDto>>>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;
        public GetBookingQueryHandler(RentalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
       

         async Task<ActionResult<List<BookingDto>>> IRequestHandler<GetBookingQuery, ActionResult<List<BookingDto>>>.Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _context.Bookings
                           .Include(b => b.Vehicle)
                           .Include(b => b.Renter)
                           .Where(b => b.RenterId == request.UserId|| b.Vehicle.OwnerId == request.UserId)
                           .OrderByDescending(b => b.CreatedAt)
                           .ToListAsync();
            if (bookings == null)
            {
                return new NotFoundResult();
            }
           
            return new OkObjectResult(_mapper.Map<BookingDto>(bookings));
        }
    }
    
    
}
