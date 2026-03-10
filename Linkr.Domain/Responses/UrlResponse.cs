namespace Linkr.Domain.Responses;

public class UrlResponse
{
	public int Id { get; set; }
	public string OriginalUrl { get; set; }
	public string ShortUrl { get; set; }
	public string UserId { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public IEnumerable<UrlClickResponse> Clicks { get; set; }
}