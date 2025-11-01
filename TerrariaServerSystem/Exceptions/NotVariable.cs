namespace TerrariaServerSystem.Exceptions;

public class NotVariable(string message) : Exception
{
    public override string Message => message;
}
