namespace Application.Dto.Review;

/// <summary>
/// Inbound DTO for review creation — contains only fields the client legitimately controls.
/// ReviewerId is NOT in this type: it is set by the controller from the authenticated user's
/// JWT identity. RevieweeId is also NOT in this type: it is derived by the handler from
/// booking participants (renter reviews host, host reviews renter). Neither can be spoofed
/// by the client.
/// </summary>
public class CreateReviewRequest
{
    public Guid BookingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
