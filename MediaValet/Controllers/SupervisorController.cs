using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using MediaValet.Application.Handlers.ConfirmationHandler.Queries;
using MediaValet.Application.Handlers.OrdersHandler;

namespace MediaValet.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SupervisorController : ControllerBase
  {
    private readonly IMediator _mediator;
    public SupervisorController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post(string orderText)
    {
      try
      {
        return Accepted(await _mediator.Send(new OrderCommand(orderText)));
      }
      catch (Exception e)
      {
        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, e.Message);
      }
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        return Ok(await _mediator.Send(new GetConfirmationsQuery()));
      }
      catch (Exception e)
      {
        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, e.Message);
      }
    }
  }
}
