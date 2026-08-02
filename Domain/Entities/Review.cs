using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Review : AggregateRoot
{
    public Guid BookingId { get; private set; }
    public Guid VehicleId { get; private set; }
    
    public Guid ReviewerId { get; private set; }
    
    public Guid RevieweeId { get; private set; }
    
    public int Rating { get; private set; }

    public string Comment { get; private set; } = string.Empty;


    public ReviewStatus Status { get; private set; }


    public DateTime CreatedAt { get; private set; }


    private Review()
    {

    }


    private Review(
        Guid bookingId,
        Guid vehicleId,
        Guid reviewerId,
        Guid revieweeId,
        int rating,
        string comment)
    {

        if(rating < 1 || rating > 5)
        {
            throw new DomainException(
                "Rating must be between 1 and 5");
        }


        BookingId = bookingId;

        VehicleId = vehicleId;

        ReviewerId = reviewerId;

        RevieweeId = revieweeId;


        Rating = rating;

        Comment = comment;


        Status = ReviewStatus.Published;

        CreatedAt = DateTime.UtcNow;
    }



    public static Review Create(
        Guid bookingId,
        Guid vehicleId,
        Guid reviewerId,
        Guid revieweeId,
        int rating,
        string comment)
    {
        if(reviewerId == revieweeId)
        {
            throw new DomainException(
                "User cannot review themselves");
        }


        return new Review(
            bookingId,
            vehicleId,
            reviewerId,
            revieweeId,
            rating,
            comment);
    }



    public void Hide()
    {
        Status = ReviewStatus.Hidden;
    }
}