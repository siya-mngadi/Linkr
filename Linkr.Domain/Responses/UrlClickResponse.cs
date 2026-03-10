namespace Linkr.Domain.Responses;

public class UrlClickResponse
{
	public int Id { get; set; }
	public int UrlId { get; set; }
	public string IpAddress { get; set; }
	public DateTime CreatedAtUtc { get; set; }
}