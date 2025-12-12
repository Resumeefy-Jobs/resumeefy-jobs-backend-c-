namespace Resumeefy.Core.Exceptions;

public class ConflictException : ApiException
{
	public ConflictException(string message) : base(message) { }
}