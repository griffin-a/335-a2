using Microsoft.AspNetCore.Mvc;

namespace A2.Controllers;

[ApiController]
[Route("api")]
public class A2Controller : Controller
{
    [HttpPost]
    public ActionResult<string> RegisterUser()
    {
        return null;
    }
}