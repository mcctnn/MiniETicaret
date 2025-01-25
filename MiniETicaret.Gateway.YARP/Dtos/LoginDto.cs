namespace MiniETicaret.Gateway.YARP.Dtos;

public sealed record LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}