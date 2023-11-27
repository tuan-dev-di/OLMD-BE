using System;
namespace OptimizingLastMile.Models.Commons;

public class ErrorObject
{
    public string ErrorCode { get; private set; }
    public string ErrorMessage { get; private set; }

    public ErrorObject(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public ErrorObject(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}
