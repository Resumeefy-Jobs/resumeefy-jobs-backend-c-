namespace Resumeefy.Core.Exceptions;

public class NotFoundException : ApiException
{
	public NotFoundException(string name, object key)
		: base($"Entity \"{name}\" ({key}) was not found.") { }
}