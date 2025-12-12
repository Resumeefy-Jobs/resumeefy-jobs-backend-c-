namespace Resumeefy.Core.Common;

public class BaseResponse<T>
{
	public bool Succeeded { get; set; }
	public string Message { get; set; } = string.Empty;
	public List<string>? Errors { get; set; }
	public T? Data { get; set; }

	public BaseResponse() { }

	public BaseResponse(T data, string message = null)
	{
		Succeeded = true;
		Message = message ?? "Success";
		Data = data;
	}

	public BaseResponse(string message)
	{
		Succeeded = false;
		Message = message;
	}

	public BaseResponse(string message, List<string> errors)
	{
		Succeeded = false;
		Message = message;
		Errors = errors;
	}
}