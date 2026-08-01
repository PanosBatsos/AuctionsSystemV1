using AuctionsSystem.AccountService.Api.DTOs.RegisterAccount;
using AuctionsSystem.AccountService.Application.Features.RegisterAccount;
using MediatR;
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

            var accountId = await _mediator.Send(command, cancellationToken);
            return Ok(new RegisterAccountResponseDto(accountId, "Account created succesfully"));
        }
    }
}
