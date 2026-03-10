namespace Linkr.Domain.Entities;

public class UrlClick
{
	public int Id { get; set; }
	public int UrlId { get; set; }
	public string IpAddress { get; set; }
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public Url Url { get; set; }
}