using Linkr.Domain.Requests.User;
using Linkr.Domain.Responses;

namespace Linkr.Domain.Services;

public interface IUserService
{
	Task LogoutAsync();
	Task<UserResponse> GetUserAsync(string email, CancellationToken cancellation = default);
	Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellation = default);
	Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellation = default);
}