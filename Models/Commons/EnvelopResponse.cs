using System;
using Microsoft.VisualBasic;

namespace OptimizingLastMile.Models.Commons;

public class EnvelopResponse
{
    public object Result { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string InvalidField { get; set; }

    private EnvelopResponse(object result, ErrorObject error, string invalidField)
    {
        Result = result;
        ErrorCode = error?.ErrorCode;
        ErrorMessage = error?.ErrorMessage;
        InvalidField = invalidField;
    }

    public static EnvelopResponse Ok(object result)
    {
        return new(result, null, null);
    }

    public static EnvelopResponse Error(ErrorObject error, string invalidField)
    {
        return new(null, error, invalidField);
    }

    public static EnvelopResponse Error(ErrorObject error)
    {
        return new(null, error, null);
    }
}

