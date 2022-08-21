using A2.data;
using A2.Data;
using A2.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class A2Repo : IA2Repo
{
    private readonly A2DBContext _dbContext;
    public A2Repo(A2DBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public User AddUser(User newUser)
    {
        EntityEntry<User> e = _dbContext.Users.Add(newUser);
        User u = e.Entity;
        _dbContext.SaveChanges();
        _dbContext.Users.Add(newUser);
        return u;
    }
}