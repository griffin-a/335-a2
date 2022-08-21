using A2.Data;
using A2.Models;
using Microsoft.AspNetCore.Mvc;

namespace A2.Controllers;

[ApiController]
[Route("api")]
public class A2Controller : Controller
{
    private readonly IA2Repo _repository;
    
    public A2Controller(IA2Repo repository)
    {
        _repository = repository;
    }
     
    [HttpPost("Register")]
    public ActionResult<string> RegisterUser(User user)
    {
        _repository.AddUser(user);
        return null;
    }
}