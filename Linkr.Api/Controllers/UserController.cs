using Asp.Versioning;
using Linkr.Domain.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Linkr.Domain.Requests.User;
using Microsoft.AspNetCore.Authorization;

namespace Linkr.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UserController : ControllerBase
{
	private readonly IUserService _service;

	public UserController(IUserService service)
	{
		this._service = service;
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
		if (claim == null) return Unauthorized();

		var user = await _service.GetUserAsync(claim.Value);
		return Ok(user);
	}

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<IActionResult> Login(SignInRequest request)
	{
		var token = await _service.SignInAsync(request);
		if (token == null) return BadRequest();
		return Ok(token);
	}

	[AllowAnonymous]
	[HttpDelete("logout")]
	public async Task<IActionResult> Logout()
	{
		await _service.LogoutAsync();
		return NoContent();
	}

	[AllowAnonymous]
	[HttpPost("sign-up")]
	public async Task<IActionResult> SignUp(SignUpRequest request)
	{
		var user = await _service.SignUpAsync(request);
		if (user == null) return BadRequest();
		return CreatedAtAction(nameof(Get), new { }, user);
	}
}