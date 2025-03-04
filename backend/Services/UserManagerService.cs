namespace backend.Services;

using backend.Model;

public class UserManagerService
{
    public List<User> LoggedUsers { get; private set; } = new List<User>();

    public void AddUser(User user)
    {
        LoggedUsers.Add(user);
    }

    public List<User> GetLoggedUsers()
    {
        return LoggedUsers;
    }
}