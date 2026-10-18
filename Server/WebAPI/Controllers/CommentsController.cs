using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;

    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepo.GetMany();
        Dictionary<int, string> userNames = GetUserNames();

        if (userId != null)
            comments = comments.Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingUserIds = userNames
                .Where(u => u.Value.ToLower().Contains(userName.ToLower()))
                .Select(u => u.Key)
                .ToList();

            comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
        }

        if (postId != null)
            comments = comments.Where(c => c.PostId == postId);

        List<CommentDto> result = comments
            .ToList()
            .Select(c => ToCommentDto(c, userNames))
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            return Ok(ToCommentDto(comment, GetUserNames()));
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto commentDto)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            comment.Body = commentDto.Body;
            await commentRepo.UpdateAsync(comment);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }


    private Dictionary<int, string> GetUserNames()
    {
        return userRepo.GetMany().ToDictionary(u => u.Id, u => u.UserName);
    }

    private static CommentDto ToCommentDto(Comment c, Dictionary<int, string> userNames)
    {
        return new CommentDto
        {
            Id = c.Id,
            Body = c.Body,
            UserId = c.UserId,
            PostId = c.PostId,
            AuthorUserName = userNames.GetValueOrDefault(c.UserId, "Unknown")
        };
    }
}