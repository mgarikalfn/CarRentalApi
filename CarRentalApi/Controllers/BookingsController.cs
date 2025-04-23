using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.Booking;
using CarRentalApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;
        public BookingsController(UserManager<ApplicationUser> userManager,RentalDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> GetBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var bookings = await _context.Bookings
                           .Include( b => b.Vehicle)
                           .Include(b => b.Renter)
                           .Where(b => b.RenterId == user.Id || b.Vehicle.OwnerId == user.Id)
                           .OrderByDescending(b => b.CreatedAt)
                           .ToListAsync();
            if (bookings == null)
            {
                return NotFound();
            }
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(b => b.Renter)
               // .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id &&
                    (b.RenterId == userId || b.Vehicle.OwnerId == userId));

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookingDto>(booking));
        }


        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            var userId = _userManager.GetUserId(User);
            var vehicle = await _context.Vehicles.FindAsync(createBookingDto.VehicleId);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found");
            }

            if (vehicle.OwnerId == userId)
            {
                return BadRequest("You cannot book your own vehicle");
            }

            // Check for date conflicts
            var isAvailable = await _context.Bookings
                .Where(b => b.VehicleId == vehicle.Id &&
                           b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Rejected)
                .AllAsync(b => createBookingDto.EndDate < b.StartDate ||
                              createBookingDto.StartDate > b.EndDate);

            if (!isAvailable)
            {
                return BadRequest("The vehicle is not available for the selected dates");
            }

            // Calculate total price
            var days = (createBookingDto.EndDate - createBookingDto.StartDate).Days;
            var totalPrice = days * vehicle.DailyPrice;

            var booking = new Booking
            {
                VehicleId = createBookingDto.VehicleId,
                RenterId = userId,
                StartDate = createBookingDto.StartDate,
                EndDate = createBookingDto.EndDate,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking),
                new { id = booking.Id },
                _mapper.Map<BookingDto>(booking));
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelBooking(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.Id == id &&
                    (b.RenterId == userId || b.Vehicle.OwnerId == userId));

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            {
                return BadRequest("Booking can't be cancelled at this stage");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptBooking(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                 .Include(b => b.Vehicle)
                 .FirstOrDefaultAsync(b => b.Id == id &&
                 b.Vehicle.OwnerId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if(!(booking.Status == BookingStatus.Pending))
            {
                return BadRequest("Booking can't be accepted at this stage");
            }

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectBooking(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.Id == id &&
                b.Vehicle.OwnerId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            

            booking.Status = BookingStatus.Rejected;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
