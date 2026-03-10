using Linkr.Domain.Entities;

namespace Linkr.Domain.Repositories;

public interface IUserRepository
{
	Task LogoutAsync();
	Task<User> GetByEmailAsync(string requestEmail, CancellationToken cancellationToken = default);
	Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken = default);
	Task<bool> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}