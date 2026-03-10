using FluentValidation;
using Linkr.Domain.Requests.Url;

namespace Linkr.Domain.Requests.Validators;

public class CreateUrlRequestValidator : AbstractValidator<CreateUrlRequest>
{
	public CreateUrlRequestValidator()
	{
		RuleFor(x => x.OriginalUrl)
			.NotNull()
			.NotEmpty()
			.WithMessage("A URL must be provided.");

		RuleFor(x => x.OriginalUrl)
			.MaximumLength(2048)
			.WithMessage("URL must be less than 2048 in length.");

		RuleFor(x => x.OriginalUrl)
			.Must(IsValidHttpsUrl)
			.WithMessage("The specified URL is invalid.");
	}

	public bool IsValidHttpsUrl(string url)
	{
		var urlValid = Uri.TryCreate(url, UriKind.Absolute, out var result);
		return urlValid && result.Scheme == Uri.UriSchemeHttps;
	}
}