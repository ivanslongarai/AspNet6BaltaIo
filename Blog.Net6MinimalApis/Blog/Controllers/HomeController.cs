using Microsoft.AspNetCore.Mvc;
using Blog.Attributes;

namespace Blog.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{

    [HttpGet("")]
    [ApiKey] // It's just an example of authenticationg by an ApiKey (Blog.Attributes.ApiKey)
    public IActionResult get()
    {
        return Ok(new
        {
            api_version = 1.1
        });
    }

}
