using Beacon.API.Helpers;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Laboratories;

[ApiController, Route("api/[controller]")]
public class LaboratoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LaboratoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLaboratory(CreateNewLaboratoryRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        return result.IsError ? result.Errors.ToValidationProblemResult() : Ok(result.Value);
    }

    [HttpGet("{userId:Guid}")]
    public async Task<IActionResult> GetMembershipsByUserId(Guid userId, CancellationToken ct)
    {
        var request = new GetMembershipsByUserIdRequest { UserId = userId };
        var result = await _mediator.Send(request, ct);
        return Ok(result.Value);
    }
}
