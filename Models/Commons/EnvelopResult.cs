using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OptimizingLastMile.Models.Commons;

public class EnvelopResult : IActionResult
{
    private readonly EnvelopResponse _envelop;
    private readonly int _statusCode;

    public EnvelopResult(EnvelopResponse envelop, HttpStatusCode statusCode)
    {
        _envelop = envelop;
        _statusCode = (int)statusCode;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(_envelop)
        {
            StatusCode = _statusCode
        };

        return objectResult.ExecuteResultAsync(context);
    }
}

