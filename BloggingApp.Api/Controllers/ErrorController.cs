using Microsoft.AspNetCore.Mvc;

namespace BloggingApp.Api.Controllers;

[ApiController]
[Route("error")]
public class ErrorController : ControllerBase
{
    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpPatch]
    public IActionResult Error()
    {
        return Problem(
            title: "An error occurred",
            detail: "An unexpected error occurred while processing your request.",
            statusCode: 500
        );
    }
}
