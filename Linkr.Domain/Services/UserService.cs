using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Linkr.Domain.Configuration;
using Linkr.Domain.Entities;
using Linkr.Domain.Mappers;
using Linkr.Domain.Repositories;
using Linkr.Domain.Requests.User;
using Linkr.Domain.Responses;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Linkr.Domain.Services;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly UserMapper _mapper = new();

	private readonly AuthenticationSettings _authenticationSettings;

	public UserService(
		IUserRepository userRepository,
		IOptions<AuthenticationSettings> settings)
	{
		_userRepository = userRepository;
		_authenticationSettings = settings.Value;
	}

	public async Task<UserResponse> GetUserAsync(string email, CancellationToken cancellation = default)
	{
		var response = await _userRepository.GetByEmailAsync(email, cancellation);
		return _mapper.ToDto(response);
	}

	public async Task LogoutAsync()
	{
		await _userRepository.LogoutAsync();
	}

	public async Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellation = default)
	{
		var user = new User()
		{
			Email = request.Email,
			Name = request.Name,
		};

		var response = await _userRepository.SignUpAsync(user, request.Password, cancellation);
		if (!response) throw new DataException("Failed to create user");
		var result = await _userRepository.GetByEmailAsync(request.Email, cancellation);
		return _mapper.ToDto(result);
	}

	public async Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellation = default)
	{
		var response = await _userRepository.AuthenticateAsync(request.Email, request.Email, cancellation);
		if (!response) throw new DataException("User not found");
		return new TokenResponse
		{
			Token = GenerateSecurityToken(request)
		};
	}

	private string GenerateSecurityToken(SignInRequest request)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_authenticationSettings.Secret);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Email, request.Email)
			}),
			Expires = DateTime.UtcNow.AddDays(_authenticationSettings.ExpirationDays),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.RsaSha256Signature)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}