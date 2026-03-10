using Microsoft.AspNetCore.Identity;

namespace Linkr.Domain.Entities;

public class User : IdentityUser
{
	public string Name { get; set; }
}