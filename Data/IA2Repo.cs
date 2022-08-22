using A2.Models;
using System.Linq;

namespace A2.Data;

public interface IA2Repo
{
    bool AddUser(User user);
    bool ValidLogin(string username, string password);
    User GetUserByUsername(string username);
    IEnumerable<GameRecord> GetGameRecords();
    GameRecord AddGameRecord(GameRecord gameRecord);
    GameRecord GetGameRecordById(Guid id);
}