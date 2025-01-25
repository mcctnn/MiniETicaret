namespace MiniETicaret.Gateway.YARP.Models;

public  sealed class User
{
    public User()
    {
        Id = Guid.NewGuid(); 
    }
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
