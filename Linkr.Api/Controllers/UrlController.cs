using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Linkr.Domain.Configuration;
using Linkr.Domain.Requests.Url;
using Linkr.Domain.Requests.Validators;
using Linkr.Domain.Services;
using System.Security.Claims;

namespace Linkr.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[EnableRateLimiting("fixed")]
[Route("api/v{version:apiVersion}/shorten")]
public class UrlController : ControllerBase
{
	private readonly IUrlService _userService;

	public readonly ShortCodeSettings _shortCodeSettings;

	private readonly CreateUrlRequestValidator _validator = new();

	public UrlController(IUrlService userService, IOptions<ShortCodeSettings> shortCodeSettings)
	{
		_userService = userService;
		_shortCodeSettings = shortCodeSettings.Value;
	}

	[AllowAnonymous]
	[HttpGet("links")]
	public async Task<IActionResult> Get()
	{
		if (!HttpContext.User.Identity?.IsAuthenticated ?? true) return Ok(await _userService.GetAnonymousUrls());
		var claims = HttpContext.User.FindFirst(ClaimTypes.Email);
		var results = await _userService.GetByUser(claims?.Value);
		return Ok(results);
	}

	[HttpGet("links/{id:int}")]
	public async Task<IActionResult> GetById(int id)
	{
		var result = await _userService.GetById(id);
		return Ok(result);
	}

	[HttpGet]
	[Route("links/codes/{code:required}")]
	public async Task<IActionResult> GetByCode(string code)
	{
		if (code.Trim().Length != _shortCodeSettings.MaxCodeLength)
		{
			return Redirect("/app");
		}

		var result = await _userService.GetByCode(code.Trim());
		if (result == null)
		{
			return NotFound("Unable to find specified code");
		}
		_ = await _userService.UpdateClicks(result.Id);
		return Ok(result);
	}

	[AllowAnonymous]
	[HttpGet]
	[Route("/{code:required}")]
	public async Task<IActionResult> RedirectByCode(string code)
	{
		if (code.Trim().Length != _shortCodeSettings.MaxCodeLength)
		{
			return Redirect("/app");
		}

		var result = await _userService.GetByCode(code.Trim());
		if (result == null)
		{
			return NotFound("Unable to find specified code");
		}
		_ = await _userService.UpdateClicks(result.Id);
		return Redirect(result.OriginalUrl);
	}

	[AllowAnonymous]
	[HttpGet("links/{id:int}/clicks")]
	public async Task<IActionResult> GetUrlClicks(int id)
	{
		var result = await _userService.GetUrlClicks(id);
		return Ok(result);
	}

	[AllowAnonymous]
	[HttpPost("links")]
	public async Task<IActionResult> CreateShortUrl(CreateUrlRequest request)
	{
		var validated = await _validator.ValidateAsync(request);
		if (!validated.IsValid) return BadRequest(validated.ToString());
		var result = await _userService.Create(request);
		return Ok(result);
	}

	[AllowAnonymous]
	[HttpPut("links/{id:int}/clicks")]
	public async Task<IActionResult> UpdateUrlClicks(int id)
	{
		var result = await _userService.UpdateClicks(id);
		return Ok(result);
	}

	[HttpDelete("links/{id:int}")]
	public IActionResult Delete(int id)
	{
		var results = _userService.Delete(id);
		if (!results) return NotFound("Unable to delete specified short url");
		return NoContent();
	}
}