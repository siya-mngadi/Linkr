namespace Linkr.Domain.Entities;

public class Url
{
	public int Id { get; set; }
	public string OriginalUrl { get; set; }
	public string Code { get; set; }
	public string ShortUrl { get; set; }
	public string UserId { get; set; }
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public User User { get; set; }
	public IEnumerable<UrlClick> Clicks { get; set; } = new List<UrlClick>();
}