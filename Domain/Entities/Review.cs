using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Review : AggregateRoot
{
    public Guid RentalId { get; private set; }

    public Guid VehicleId { get; private set; }


    public Guid ReviewerId { get; private set; }

    public Guid ReceiverId { get; private set; }


    public int Rating { get; private set; }

    public string Comment { get; private set; } = string.Empty;


    public ReviewStatus Status { get; private set; }


    public DateTime CreatedAt { get; private set; }


    private Review()
    {

    }


    private Review(
        Guid rentalId,
        Guid vehicleId,
        Guid reviewerId,
        Guid receiverId,
        int rating,
        string comment)
    {

        if(rating < 1 || rating > 5)
        {
            throw new DomainException(
                "Rating must be between 1 and 5");
        }


        RentalId = rentalId;

        VehicleId = vehicleId;

        ReviewerId = reviewerId;

        ReceiverId = receiverId;


        Rating = rating;

        Comment = comment;


        Status = ReviewStatus.Published;

        CreatedAt = DateTime.UtcNow;
    }



    public static Review Create(
        Guid rentalId,
        Guid vehicleId,
        Guid reviewerId,
        Guid receiverId,
        int rating,
        string comment)
    {
        if(reviewerId == receiverId)
        {
            throw new DomainException(
                "User cannot review themselves");
        }


        return new Review(
            rentalId,
            vehicleId,
            reviewerId,
            receiverId,
            rating,
            comment);
    }



    public void Hide()
    {
        Status = ReviewStatus.Hidden;
    }
}