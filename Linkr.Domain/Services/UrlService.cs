using System.Text;
using System.Buffers.Text;
using Linkr.Domain.Mappers;
using System.Security.Claims;
using Linkr.Domain.Entities;
using Linkr.Domain.Responses;
using Microsoft.AspNetCore.Http;
using Linkr.Domain.Requests.Url;
using Linkr.Domain.Repositories;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Linkr.Infrastructure.Configuration;
using Linkr.Domain.Configuration;

namespace Linkr.Domain.Services;

public class UrlService : IUrlService
{
	private readonly IUrlRepository _urlRepository;
	private readonly IUrlClickRepository _urlClickRepository;
	private readonly IHttpContextAccessor _httpContextAccessor;

	private readonly UrlMapper _mapper = new();
	private readonly WebsiteConfiguration _websiteConfig;
	private readonly ShortCodeSettings _shortCodeSettings;

	public UrlService(
		IUrlRepository urlRepository,
		IUrlClickRepository urlClickRepository,
		IOptions<WebsiteConfiguration> websiteConfig,
		IOptions<ShortCodeSettings> shortCodeSettings,
		IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
		_urlRepository = urlRepository;
		_urlClickRepository = urlClickRepository;
		_websiteConfig = websiteConfig.Value;
		_shortCodeSettings = shortCodeSettings.Value;
	}

	public async Task<IEnumerable<UrlResponse>> GetAnonymousUrls()
	{
		var results = await _urlRepository.GetAnonymousUrls();
		return _mapper.ToDto(results);
	}

	public async Task<IEnumerable<UrlResponse>> GetByUser(string userId)
	{
		var results = await _urlRepository.GetByUser(userId);
		return _mapper.ToDto(results);
	}

	public async Task<UrlClickStats> GetUrlClicks(int urlId)
	{
		var urlClicks = await _urlClickRepository.GetByUrl(urlId);
		var clicks = urlClicks.ToList();
		return new UrlClickStats
		{
			UrlId = urlId,
			Clicks = clicks.Count,
			LastClicked = clicks.MaxBy(x => x.CreatedAtUtc).CreatedAtUtc,
		};
	}

	public async Task<UrlResponse> GetByCode(string code)
	{
		var result = await _urlRepository.GetByCode(code);
		return _mapper.ToDto(result);
	}

	public async Task<UrlResponse> GetById(int id)
	{
		var result = await _urlRepository.GetById(id);
		return _mapper.ToDto(result);
	}

	public async Task<UrlResponse> Create(CreateUrlRequest request)
	{
		var details = _mapper.FromDto(request);
		details.UserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
		details.Code = await GenerateUniqueCode();
		details.ShortUrl = GenerateShortUrl(details.Code);
		var result = await _urlRepository.Create(details);
		await _urlRepository.UnitOfWork.SaveChangesAsync();
		return _mapper.ToDto(result);
	}

	public async Task<UrlClickStats> UpdateClicks(int urlId)
	{
		var url = await GetById(urlId);
		var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
		_ = await _urlClickRepository.Create(new UrlClick { UrlId = url.Id, IpAddress = ipAddress });
		await _urlClickRepository.UnitOfWork.SaveChangesAsync();
		return await GetUrlClicks(urlId);
	}

	public bool Delete(int id)
	{
		return _urlRepository.Delete(id);
	}

	private string GenerateShortUrl(string code)
	{
		var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin.ToString();
		return $"{_websiteConfig.Url ?? origin}/{code}";
	}

	private async Task<string> GenerateUniqueCode()
	{
		string code;
		do
		{
			var guid = Guid.NewGuid();
			var timestamp = DateTime.UtcNow.Ticks;
			var combined = $"{timestamp}{guid}";
			var hash = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
			var base64 = Base64Url.EncodeToString(hash);
			code = base64[..Math.Min(_shortCodeSettings.MaxCodeLength, base64.Length)];
		} while (await CodeExistsInDatabase(code));
		return code;
	}

	private async Task<bool> CodeExistsInDatabase(string code)
	{
		var uniqueCodes = (await _urlRepository.GetUrls()).AsParallel().Select(x => x.Code).ToHashSet();
		return uniqueCodes.Contains(code);
	}
}