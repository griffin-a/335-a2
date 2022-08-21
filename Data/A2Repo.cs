using A2.data;
using A2.Data;
using A2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class A2Repo : IA2Repo
{
    private readonly A2DBContext _dbContext;
    public A2Repo(A2DBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public bool AddUser(User newUser)
    {
        if (_dbContext.Users.SingleOrDefault(u => u.UserName == newUser.UserName) == null) return false;
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
        return true;

    }
}