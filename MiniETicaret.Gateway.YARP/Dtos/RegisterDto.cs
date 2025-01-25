namespace MiniETicaret.Gateway.YARP.Dtos;

public sealed record RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}
