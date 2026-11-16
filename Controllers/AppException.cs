namespace Lab3.Controllers;

/// <summary>A business-rule error whose message is shown to the user as it is.</summary>
public class AppException : Exception
{
    public AppException(string message) : base(message) { }
}
