namespace backend.Model;

public class User
{
    public string Email { get; set; }
    public string[] Permissions { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }

    public User(string email, string[] permissions, string password, string[] roles)
    {
        Email = email;
        Permissions = permissions;
        Password = password;
        Roles = roles;
    }
}