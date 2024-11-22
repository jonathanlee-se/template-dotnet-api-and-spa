using Microsoft.AspNetCore.Mvc;

namespace SE.Sustainability.Template.Controllers;

/// <summary>
/// Example hello world controller
/// </summary>
[Route("hello")]
public class HelloWorldController : Controller
{
    [HttpGet]
    public IActionResult Greet()
    {
        return Ok("Hello, World");
    }
}