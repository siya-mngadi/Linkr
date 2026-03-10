namespace Linkr.Api.Middleware;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionMiddleware> _logger;
	public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			_logger.LogInformation(
				"Incoming request: {Method} {Path}{QueryString}",
				context.Request.Method,
				context.Request.Path.Value,
				context.Request.QueryString.Value
			);

			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await context.Response.WriteAsync("Internal server error");
		}
	}
}