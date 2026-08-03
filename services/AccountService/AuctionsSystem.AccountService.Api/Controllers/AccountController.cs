using AuctionsSystem.AccountService.Api.DTOs.LoginAccount;
using AuctionsSystem.AccountService.Api.DTOs.RegisterAccount;
using AuctionsSystem.AccountService.Application.Features.LoginAccount;
using AuctionsSystem.AccountService.Application.Features.RegisterAccount;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuctionsSystem.AccountService.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountRequestDto request, CancellationToken cancellationToken)
        {
            var command = new RegisterAccountCommand(
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password,
                request.IdNumber,
                request.PhoneNumber
            );

            var handlerResponse = await _mediator.Send(command, cancellationToken);
            return Ok(new RegisterAccountResponseDto(handlerResponse.Id,
                handlerResponse.Email,
                handlerResponse.PhoneNumber,
                "Account created successfully"));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginAccountRequestDto request, CancellationToken cancellationToken) 
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var command = new LoginAccountCommand(request.Email, request.Password, ipAddress);

            var handlerResponse = await _mediator.Send(command, cancellationToken);
            return Ok(new LoginAccountResponseDto(handlerResponse.Username,
                handlerResponse.Email,
                handlerResponse.Token,
                "User logged in successfully"));
        }
    }
}
