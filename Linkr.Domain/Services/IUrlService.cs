using Linkr.Domain.Requests.Url;
using Linkr.Domain.Responses;

namespace Linkr.Domain.Services;

public interface IUrlService
{
	Task<IEnumerable<UrlResponse>> GetAnonymousUrls();
	Task<IEnumerable<UrlResponse>> GetByUser(string userId);
	Task<UrlClickStats> GetUrlClicks(int urlId);
	Task<UrlResponse> GetByCode(string shortUrl);
	Task<UrlResponse> GetById(int id);
	Task<UrlResponse> Create(CreateUrlRequest request);
	Task<UrlClickStats> UpdateClicks(int urlId);
	bool Delete(int id);
}