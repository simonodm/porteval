using System.IO;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core;

namespace PortEval.Application.Controllers;

public abstract class PortEvalControllerBase : ControllerBase
{
    protected ActionResult<T> GenerateActionResult<T>(OperationResponse<T> response)
    {
        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(response.Message);
        }

        if (response.Status == OperationStatus.Error)
        {
            return BadRequest(response.Message);
        }

        return response.Response;
    }

    protected ActionResult<T> GenerateActionResult<T>(OperationResponse<T> response, string actionName,
        object routeValues)
    {
        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(response.Message);
        }

        if (response.Status == OperationStatus.Error)
        {
            return BadRequest(response.Message);
        }

        return CreatedAtAction(actionName, routeValues, response.Response);
    }

    protected IActionResult GenerateActionResult(OperationResponse response)
    {
        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(response.Message);
        }

        if (response.Status == OperationStatus.Error)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }

    protected IActionResult GenerateFileActionResult(OperationResponse<Stream> response, string contentType,
        string fileName)
    {
        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(response.Message);
        }

        if (response.Status == OperationStatus.Error)
        {
            return BadRequest(response.Message);
        }

        return File(response.Response, contentType, fileName);
    }

    protected IActionResult GenerateFileActionResult(OperationResponse<byte[]> response, string contentType,
        string fileName)
    {
        if (response.Status == OperationStatus.NotFound)
        {
            return NotFound(response.Message);
        }

        if (response.Status == OperationStatus.Error)
        {
            return BadRequest(response.Message);
        }

        return File(response.Response, contentType, fileName);
    }
}