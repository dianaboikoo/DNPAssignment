namespace DTOs;

public record PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public int UserId { get; set; }
    public string AuthorUserName { get; set; } = "";
    public List<CommentDto>? Comments { get; set; }
}

public record CreatePostDto
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public int UserId { get; set; }
}

public record UpdatePostDto
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";

}