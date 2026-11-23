using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;

    public CommentsController(ICommentRepository commentRepo)
    {
        this.commentRepo = commentRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepo.GetMany();

        if (userId != null)
            comments = comments.Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(userName))
            comments = comments.Where(c => c.User.UserName.ToLower().Contains(userName.ToLower()));

        if (postId != null)
            comments = comments.Where(c => c.PostId == postId);

        List<CommentDto> result = await comments
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                PostId = c.PostId,
                AuthorUserName = c.User.UserName
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        CommentDto? dto = await commentRepo.GetMany()
            .Where(c => c.Id == id)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                PostId = c.PostId,
                AuthorUserName = c.User.UserName
            })
            .FirstOrDefaultAsync();

        if (dto == null)
            return NotFound($"Comment with id {id} not found");

        return Ok(dto);
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
}