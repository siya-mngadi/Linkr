using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Linkr.Domain.Entities;
using Linkr.Domain.Repositories;

namespace Linkr.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
	private readonly SignInManager<User> _signInManager;
	private readonly UserManager<User> _userManager;

	public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public async Task<bool> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
	{
		var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
		return result.Succeeded;
	}

	public async Task LogoutAsync()
	{
		await _signInManager.SignOutAsync();
	}

	public async Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken = default)
	{
		var result = await _userManager.CreateAsync(user, password);
		return result.Succeeded;
	}

	public async Task<User> GetByEmailAsync(string requestEmail, CancellationToken cancellationToken = default)
	{
		return await _userManager.Users.FirstOrDefaultAsync(x => x.Email == requestEmail, cancellationToken);
	}
}