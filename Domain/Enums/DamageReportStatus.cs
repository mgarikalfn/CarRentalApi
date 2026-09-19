namespace Domain.Enums;

public enum DamageReportStatus
{
    Open             = 1,
    UnderReview      = 2,
    ResolvedAtFault  = 3,
    ResolvedNotAtFault = 4,
    Dismissed        = 5
}
