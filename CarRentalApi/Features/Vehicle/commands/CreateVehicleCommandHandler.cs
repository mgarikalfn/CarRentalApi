using Application.Common;
using Application.Features.Vehicle.Command;
using MediatR;

namespace CarRentalApi.Features.Vehicle.commands;

// This file is intentionally minimal — Vehicle creation is handled
// by Application.Features.Vehicle.Command.CreateVehicleCommandHandler via MediatR.
// No separate API-layer handler needed; the controller sends CreateVehicleCommand directly.
