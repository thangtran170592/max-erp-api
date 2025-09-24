namespace Application.Common.Exceptions;
public class NotFoundException(string message, object key) : Exception($"Entity \"{message}\" ({key}) was not found.")
{
}