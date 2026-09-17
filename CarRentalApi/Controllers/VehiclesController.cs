using System.Security.Claims;
using Application.Common;
using Application.Dto.vehicle;
using Application.Features.Vehicle.Command;
using Application.Features.Vehicle;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VehiclesController(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IMediator mediator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateVehicle([FromForm] CreateVehicleDto createVehicleDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var command = _mapper.Map<CreateVehicleCommand>(createVehicleDto);
        command.OwnerId = userId;

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<VehicleDto>> GetVehicle(Guid id)
    {
        var query = new GetVehicleByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleDto vehicleUpdateDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var command = _mapper.Map<UpdateVehicleCommand>(vehicleUpdateDto);
        command.OwnerId = user.Id;
        command.Id = id;

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var command = new DeleteVehicleCommand { Id = id, OwnerId = user.Id.ToString() };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
