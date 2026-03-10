using Riok.Mapperly.Abstractions;
using Linkr.Domain.Entities;
using Linkr.Domain.Responses;

namespace Linkr.Domain.Mappers;

[Mapper]
public partial class UserMapper
{
	public partial UserResponse ToDto(User user);
}