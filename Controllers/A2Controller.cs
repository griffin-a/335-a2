using System.Resources;
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

    [HttpGet("PairMe")]
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

        GameRecordOut gameRecordOut;

        if (queuedGame == null)
        { 
           GameRecord newGame = new GameRecord { Player1 = username };
           _repository.AddGameRecord(newGame);
           gameRecordOut = new GameRecordOut
           {
               GameId = newGame.GameId, State = newGame.State, Player1 = newGame.Player1,
               Player2 = newGame.Player2, LastMovePlayer1 = newGame.LastMovePlayer1,
               LastMovePlayer2 = newGame.LastMovePlayer2
           };

           return Ok(gameRecordOut);
        }

        
        // Check whether or not current user is Player 1 or Player 2
        if (queuedGame.Player1 != username)
        {
            queuedGame.State = "progress";
            queuedGame.Player2 = username;
        }

        gameRecordOut = new GameRecordOut
        {
            GameId = queuedGame.GameId, State = queuedGame.State, Player1 = queuedGame.Player1,
            Player2 = queuedGame.Player2, LastMovePlayer1 = queuedGame.LastMovePlayer1,
            LastMovePlayer2 = queuedGame.LastMovePlayer2
        };

        _repository.UpdateGameRecord();

        // _repository.AddGameRecord(queuedGame);
    

        return Ok(gameRecordOut);
    }

    [HttpGet("TheirMove/{gameId}")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> GetOpponentMove(string gameId)
    {
        // Check if a game exists based on the given id first
        var game = _repository.GetGameRecordById(gameId);
        Console.WriteLine(game.GameId);

        // Check if the game is a game that is played by the user (Player 1 matches the current user)
        // Check if logged in user is player 1 or player 2
        string username = GetCurrentUsername();

        if (username == game.Player1)
        {
            if(game.LastMovePlayer2 == null) return Ok("Your opponent has not moved yet.");
            if(game.LastMovePlayer2 != null) return Ok(game.LastMovePlayer2);
            if (game.Player2 == null) return Ok("You do not have an opponent yet!");
        }
        else if (username == game.Player2)
        {
            if(game.LastMovePlayer1 == null) return Ok("Your opponent has not moved yet.");
            if (game.LastMovePlayer1 != null) return Ok(game.LastMovePlayer1);
            if (game.Player1 == null) return Ok("You do not have an opponent yet!");
        }

        return Ok("not your game id");
    }

    [HttpPost("MyMove")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> MakeMove(GameMove move)
    {
        // Check if a game exists based on the given id first
        var game = _repository.GetGameRecordById(move.GameId);
        var res = "no such gameId";
        var username = GetCurrentUsername();

        if (game == null) return Ok(res);
        // Check that the game specified in the move corresponds to a game the user is assigned to
        var assignedPlayer1 = game.Player1;
        var assignedPlayer2 = game.Player2;

        // Case where user is player 1
        if (assignedPlayer1 == username)
        {
            // Check that the game's status is "progress"
            // Also check that the user's last move is null
            if (game.State == "progress")
            {
                if (game.LastMovePlayer1 == null)
                {
                    game.LastMovePlayer2 = null;
                    game.LastMovePlayer1 = move.Move;
                    _repository.UpdateGameRecord();
                    res = "move registered";
                }
                else res = "It is not your turn.";
            }
            else res = "You do not have an opponent yet.";
        }
        // Case where user is player 2
        else if (assignedPlayer2 == username)
        {
            // Check that the game's status is "progress"
            if (game.State == "progress")
            {
                if (game.LastMovePlayer2 == null)
                {
                    game.LastMovePlayer1 = null;
                    game.LastMovePlayer2 = move.Move;
                    _repository.UpdateGameRecord();
                    res = "move registered";
                }
                else res = "It is not your turn.";
            }
            else res = "You do not have an opponent yet.";
        }
        else
        {
            res = "not your game id";
        }

        return Ok(res);
    }

    [HttpPost("QuitGame/{gameId}")]
    [Authorize(AuthenticationSchemes = "MyAuthentication")]
    [Authorize(Policy = "UserOnly")]
    public ActionResult<string> QuitGame(string gameId)
    {
        // Get the game based on the passed in gameId
        var game = _repository.GetGameRecordById(gameId);
        var username = GetCurrentUsername();
        var res = "no such gameId";

        // Get username of currently logged in user
        if (game == null) return Ok(res);
        if (username == game.Player1 || username == game.Player2)
        {
            // Delete the game record from the db 
            _repository.RemoveGameRecord(game);
            res = "game over";
        }
        else
        {
            res = "You have not started a game.";
        }

        return Ok(res);

        // Check if the user is assigned to a game 
        // Check if the game is assigned to the user
    }

    
    private string GetCurrentUsername()
    {
        ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault()!;
        Claim c = ci.FindFirst("userName")!;
        return c.Value;
    }
}