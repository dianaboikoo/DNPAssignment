namespace Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public List<Post> Posts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}