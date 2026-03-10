using Riok.Mapperly.Abstractions;
using Linkr.Domain.Entities;
using Linkr.Domain.Requests.Url;
using Linkr.Domain.Responses;

namespace Linkr.Domain.Mappers;

[Mapper]
public partial class UrlMapper
{
	public partial UrlResponse ToDto(Url url);
	public partial IEnumerable<UrlResponse> ToDto(IEnumerable<Url> urls);
	public partial Url FromDto(CreateUrlRequest url);
	public partial UrlClickResponse ToDto(UrlClick url);
}