using Domain.Enums;

namespace Application.Dto.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid RevieweeId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public ReviewStatus Status { get; set; }
    public bool IsFlaggedForReview { get; set; }
    public DateTime? FlaggedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
