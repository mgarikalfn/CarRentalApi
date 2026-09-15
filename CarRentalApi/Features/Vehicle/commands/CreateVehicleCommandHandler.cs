
using MediatR;

namespace Application.Features.Vehicle.commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    Guid OwnerId,
    string Title,
    string Description,
    string Brand,
    string Model,
    int Year,
    string Color,
    FuelType FuelType,
    TransmissionType Transmission,
    string Vin,
    string LicensePlate,
    int SeatCount,
    decimal DailyPrice,
    string Currency,
    int Mileage,
    double Latitude,
    double Longitude,
    string City
) : IRequest<CreateVehicleResponse>;
public class CreateVehicleCommandHandler :IRequestHandler<CreateVehicleCommand, CreateVehicleResponse>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateVehicleResponse> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
       
    }
}