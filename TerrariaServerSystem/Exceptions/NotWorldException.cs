namespace TerrariaServerSystem.Exceptions;

public class NotWorldException : Exception
{
    private string mesg { get; init; }
    public NotWorldException(string message)
    {
        mesg = message;
    }

    public override string Message => mesg;
}
