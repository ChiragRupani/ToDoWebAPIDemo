using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ToDoWebAPI.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [Route("/error-local-development")]
    [HttpGet, HttpPost]
    public IActionResult ErrorLocalDevelopment(
        [FromServices] IWebHostEnvironment webHostEnvironment)
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return Problem(
            detail: context.Error.StackTrace,
            title: context.Error.Message);
    }

    [Route("/error")]
    [HttpGet, HttpPost]
    public IActionResult Error() => Problem();
}
