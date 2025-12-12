using Resumeefy.Core.Common;
using Resumeefy.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace Resumeefy.API.Middlewares;

public class ErrorHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ErrorHandlerMiddleware> _logger;

	public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception error)
		{
			await HandleExceptionAsync(context, error);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception error)
	{
		var response = context.Response;
		response.ContentType = "application/json";

		var responseModel = new BaseResponse<string>() { Succeeded = false, Message = error.Message };

		switch (error)
		{
			case ValidationException e:
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				responseModel.Errors = e.Errors;
				break;

			case NotFoundException e:
				response.StatusCode = (int)HttpStatusCode.NotFound;
				break;

			case ConflictException e:
				response.StatusCode = (int)HttpStatusCode.Conflict;
				break;

			case ApiException e:
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				break;

			// 3. System Exceptions
			default:
				_logger.LogError(error, error.Message);
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				responseModel.Message = "Internal Server Error. Please try again later.";
				break;
		}

		var result = JsonSerializer.Serialize(responseModel);
		await response.WriteAsync(result);
	}
}