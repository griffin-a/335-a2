using A2.Data;
using A2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace A2.Data;

public class A2Repo : IA2Repo
{
    private readonly A2DBContext _dbContext;

    public A2Repo(A2DBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool AddUser(User newUser)
    {
        if (_dbContext.Users.SingleOrDefault(u => u.UserName == newUser.UserName) != null) return false;
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
        return true;
    }

    public bool ValidLogin(string username, string password)
    {
        return _dbContext.Users.FirstOrDefault(u => u.UserName == username && u.Password == password) != null;
    }

    public User GetUserByUsername(string username) => _dbContext.Users.FirstOrDefault(u => u.UserName == username)!;

    public IEnumerable<GameRecord> GetGameRecords() => _dbContext.GameRecords.ToList();

    public GameRecord AddGameRecord(GameRecord gameRecord)
    {
        EntityEntry<GameRecord> e = _dbContext.GameRecords.Add(gameRecord);
        GameRecord g = e.Entity;
        _dbContext.SaveChanges();
        _dbContext.GameRecords.Add(gameRecord);
        return g;
    }

    public GameRecord GetGameRecordById(Guid id) => _dbContext.GameRecords.FirstOrDefault(g => g.GameId == id)!;
    public GameRecord RemoveGameRecord(GameRecord g)
    {
        // GameRecord g = _dbContext.GameRecords.FirstOrDefault(g => g.GameId == id)!;
        _dbContext.Remove(g);
        _dbContext.SaveChanges();
        return g;
    }

    public void UpdateGameRecord()
    {
        // _dbContext.GameRecords.Update(g);
        _dbContext.SaveChanges();
        // return g;
    }
}