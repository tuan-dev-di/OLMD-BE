using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualBasic;
using System.Net;
using OptimizingLastMile.Models.Commons;

namespace OptimizingLastMile.Utils;

public class ModelStateValidator
{
    public static IActionResult ValidateModelState(ActionContext context)
    {
        (string fieldName, ModelStateEntry entry) = context.ModelState.First(x => x.Value.Errors.Count > 0);

        string errorMessage = entry.Errors.First().ErrorMessage;

        var errorObject = new ErrorObject(errorMessage);

        var envelopResponse = EnvelopResponse.Error(errorObject, fieldName);
        var envelopResult = new EnvelopResult(envelopResponse, HttpStatusCode.BadRequest);

        return envelopResult;
    }
}

