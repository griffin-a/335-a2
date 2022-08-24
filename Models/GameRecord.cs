using System.ComponentModel.DataAnnotations;

namespace A2.Models;
public class GameRecord
{
    [Key]
    public int Id { get; set; }

    public string GameId { get; set; } = Guid.NewGuid().ToString();
    public string? State { get; set; } = "wait";
    public string Player1 { get; set; }
    public string? Player2 { get; set; } = null;
    public string? LastMovePlayer1 { get; set; } = null;
    public string? LastMovePlayer2 { get; set; } = null;
}