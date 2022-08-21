using A2.Models;

namespace A2.Data;

public interface IA2Repo
{
    bool AddUser(User user);
    bool ValidLogin(string username, string password);
    User GetUserByUsername(string username);
}