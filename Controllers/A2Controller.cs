using A2.Data;
using A2.Dtos;
using A2.Models;
using Microsoft.AspNetCore.Authorization;
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
    public ActionResult<string> Register(User user)
    {
        return Ok(_repository.AddUser(user) ? "User successfully registered." : "Username not available.");
    }

    [HttpGet("GetVersionA")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> GetVersion() => Ok("1.0.0 (auth)");

    [HttpPost("PurchaseItem/{itemId}")]
    public ActionResult<Order> PurchaseItem(int itemId)
    {
        return null;
    }

    [HttpPost("PairMe")]
    public ActionResult<GameRecordOut> StartGame()
    {
        return null;
    }

    [HttpGet("TheirMove/{gameId}")]
    public ActionResult<GameMove> GetOpponentMove(int gameId)
    {
        return null;
    }

    [HttpPost("MyMove")]
    public ActionResult<string> MakeMove(GameMove move)
    {
        return null;
    }

    [HttpPost("QuitGame/{gameId}")]
    public ActionResult<string> QuitGame(int gameId)
    {
        return null;
    }
}