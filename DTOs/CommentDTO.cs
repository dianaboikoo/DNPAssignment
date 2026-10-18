namespace DTOs;

public class CreateCommentDto
{   
    public string Body { get; set; } = "";
    public int UserId { get; set; } 
}

public class UpdateCommentDto
{
    public string Body { get; set; } = "";
}

public class CommentDto
{
    public int Id { get; set; }
    public string Body { get; set; } = "";
    public int UserId { get; set; }
    public int PostId { get; set; }
    public string AuthorUserName { get; set; } = "";
}
