using AuctionsSystem.AccountService.Api.DTOs.LoginAccount;
using AuctionsSystem.AccountService.Api.DTOs.RegisterAccount;
using AuctionsSystem.AccountService.Api.DTOs.UpdateAccount.UpdateUsername;
using AuctionsSystem.AccountService.Application.Features.GetAccount;
using AuctionsSystem.AccountService.Application.Features.LoginAccount;
using AuctionsSystem.AccountService.Application.Features.LogoutAccount;
using AuctionsSystem.AccountService.Application.Features.RegisterAccount;
using AuctionsSystem.AccountService.Application.Features.UpdateAccount.UpdateUsername;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            return StatusCode(StatusCodes.Status201Created, new RegisterAccountResponseDto(handlerResponse.Id,
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

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetAccount(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var query = new GetAccountQuery(userId);
            var handlerResponse = await _mediator.Send(query, cancellationToken);

            return Ok(handlerResponse);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? tokenIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Jti) ?? null;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId) || string.IsNullOrEmpty(tokenIdClaim))
            {
                return Unauthorized();
            }

            await _mediator.Send(new LogoutAccountCommand(userId, tokenIdClaim));

            return Ok();
        }

        [HttpPatch("profile/username")]
        [Authorize]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameRequestDto request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            await _mediator.Send(new UpdateUsernameCommand(userId, request.NewUsername), cancellationToken);

            return Ok("Username changed successfully");
        }
    }
}
