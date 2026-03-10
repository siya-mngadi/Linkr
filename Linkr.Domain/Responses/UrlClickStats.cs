namespace Linkr.Domain.Responses;

public class UrlClickStats
{
	public int UrlId { get; set; }
	public int Clicks { get; set; }
	public DateTime LastClicked { get; set; }
}