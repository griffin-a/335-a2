using System.Security.Claims;
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
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<Order> PurchaseItem(int itemId)
    {
        // ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault()!;
        // Claim c = ci.FindFirst("userName")!;
        // string username = c.Value;
        var user = _repository.GetUserByUsername(GetCurrentUsername());
        
        Order order = new Order
        {
            ProductId = itemId,
            UserName = user.UserName
        };
        
        return Ok(order);
    }

    [HttpPost("PairMe")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<GameRecordOut> StartGame()
    {
        // Get the current user that is logged in
        // ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault()!;
        // Claim c = ci.FindFirst("userName")!;
        string username = GetCurrentUsername();
        // var user = _repository.GetUserByUsername(username);
        
        // Check that there isn't a user waiting to play a game (no game in state "wait")
        var games = _repository.GetGameRecords();
        var queuedGame = games.FirstOrDefault(g => g.State == "wait");

        GameRecord gameRecord;
        
        if (queuedGame == null)
        { 
            gameRecord = new GameRecord { GameId = Guid.NewGuid(), State = "wait", Player1 = username };
        }
        else
        {
            gameRecord = new GameRecord { GameId = Guid.NewGuid(), State = "progress", Player2 = username };

        }

        _repository.AddGameRecord(gameRecord);

        GameRecordOut gameRecordOut = new GameRecordOut
        {
            GameId = gameRecord.GameId, State = gameRecord.State, Player1 = gameRecord.Player1,
            Player2 = gameRecord.Player2, LastMovePlayer1 = gameRecord.LastMovePlayer1,
            LastMovePlayer2 = gameRecord.LastMovePlayer2
        };
        
        return Ok(gameRecordOut);
    }

    [HttpGet("TheirMove/{gameId}")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> GetOpponentMove(Guid gameId)
    {
        // Check if a game exists based on the given id first
        var game = _repository.GetGameRecordById(gameId);
        string res = "no such gameId";

        if (game == null) return Ok(res);
        // Check if the game is a game that is played by the user (Player 1 matches the current user)
        string username = GetCurrentUsername();
        if (game.Player1 != username) res = "not your game id";
        else if (game.Player2 == null) res = "You do not have an opponent yet.";
        else if (game.LastMovePlayer2 == null) res = "Your opponent has not moved yet.";

        return Ok(res);
    }

    [HttpPost("MyMove")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> MakeMove(GameMove move)
    {
        // Check if a game exists based on the given id first
        var game = _repository.GetGameRecordById(move.GameId);
        string res = "no such gameId";

        if (game == null) return Ok(res);
        if (game.State == "wait") res = "You do not have an opponent yet.";
        else if (game.LastMovePlayer1 != null) res = "It is not your turn.";
        else 
        {
            res = "move registered.";
        }


        return Ok(res);
    }

    [HttpPost("QuitGame/{gameId}")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> QuitGame(int gameId)
    {
        return null;
    }

    private string GetCurrentUsername()
    {
        ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault()!;
        Claim c = ci.FindFirst("userName")!;
        return c.Value;
    }
}