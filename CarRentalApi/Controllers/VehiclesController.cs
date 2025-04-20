using System.Security.Claims;
using AutoMapper;
using CarRentalApi.Application.Vehicle.command;
using CarRentalApi.Application.Vehicle.query;
using CarRentalApi.Data;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;
using CarRentalApi.Service;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly RentalDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VehiclesController(
        RentalDbContext context,
        UserManager<ApplicationUser> userManager,
        IFileStorageService fileStorageService,
        IMapper mapper,
        IMediator mediator)
    {
        _context = context;
        _userManager = userManager;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _mediator = mediator;

    }

    [HttpPost]
    //[Authorize(Policy = "VehicleOwner")]
    public async Task<ActionResult<Vehicle>> CreateVehicle(
        [FromForm] CreateVehicleDto createVehicleDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var command = _mapper.Map<CreateVehicleCommand>(createVehicleDto);
        command.OwnerId = user.Id;

        var result =await  _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var command = new GetVehicleByIdCommand
        {
            Id = id
        };

        var result = await _mediator.Send(command);
        if (result == null) return NotFound();
        return Ok(result);
    }
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateVehicle([FromQuery] int id, UpdateVehicleDto vehicleUpdateDto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return NotFound();

        var command = _mapper.Map<UpdateVehicleCommand>(vehicleUpdateDto);
        command.OwnerId = user.Id;
        command.Id = id;
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteVehicle(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if(user == null) { return  NotFound(); }

        var command = new DeleteVehicleCommand
        {
            Id = id,
            OwnerId = user.Id
        };
        var result = await _mediator.Send(command);
        return Ok(result);

    }
}