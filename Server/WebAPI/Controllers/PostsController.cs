using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepo.GetMany();
        Dictionary<int, string> userNames = GetUserNames();

        if (!string.IsNullOrWhiteSpace(title))
            posts = posts.Where(p => p.Title.ToLower().Contains(title.ToLower()));

        if (userId != null)
            posts = posts.Where(p => p.UserId == userId);

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingUserIds = userNames
                .Where(u => u.Value.ToLower().Contains(userName.ToLower()))
                .Select(u => u.Key)
                .ToList();

            posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
        }

        List<PostDto> result = posts
            .ToList()
            .Select(p => ToPostDto(p, userNames))
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost(int id, [FromQuery] bool includeComments = false)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            Dictionary<int, string> userNames = GetUserNames();

            PostDto dto = ToPostDto(post, userNames);

            if (includeComments)
            {
                dto.Comments = commentRepo.GetMany()
                    .Where(c => c.PostId == id)
                    .ToList()
                    .Select(c => ToCommentDto(c, userNames))
                    .ToList();
            }

            return Ok(dto);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto postDto)
    {
        User? author = userRepo.GetMany().FirstOrDefault(u => u.Id == postDto.UserId);
        if (author is null)
            return BadRequest($"User with id {postDto.UserId} does not exist");

        Post post = new Post { Title = postDto.Title, Body = postDto.Body, UserId = postDto.UserId };
        Post created = await postRepo.AddAsync(post);

        PostDto dto = new PostDto
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId,
            AuthorUserName = author.UserName
        };
        return Created($"/posts/{dto.Id}", dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto postDto)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            post.Title = postDto.Title;
            post.Body = postDto.Body;
            await postRepo.UpdateAsync(post);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);

            List<Comment> comments = commentRepo.GetMany().Where(c => c.PostId == id).ToList();
            foreach (Comment comment in comments)
                await commentRepo.DeleteAsync(comment.Id);

            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }


    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(int id, [FromBody] CreateCommentDto commentDto)
    {
        try
        {
            await postRepo.GetSingleAsync(id);  
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }

        User? author = userRepo.GetMany().FirstOrDefault(u => u.Id == commentDto.UserId);
        if (author is null)
            return BadRequest($"User with id {commentDto.UserId} does not exist");

        Comment comment = new Comment { Body = commentDto.Body, UserId = commentDto.UserId, PostId = id };
        Comment created = await commentRepo.AddAsync(comment);

        CommentDto dto = new CommentDto
        {
            Id = created.Id,
            Body = created.Body,
            UserId = created.UserId,
            PostId = created.PostId,
            AuthorUserName = author.UserName
        };
        return Created($"/comments/{dto.Id}", dto);
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsOfPost(int id)
    {
        try
        {
            await postRepo.GetSingleAsync(id);
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }

        Dictionary<int, string> userNames = GetUserNames();

        List<CommentDto> result = commentRepo.GetMany()
            .Where(c => c.PostId == id)
            .ToList()
            .Select(c => ToCommentDto(c, userNames))
            .ToList();

        return Ok(result);
    }


    private Dictionary<int, string> GetUserNames()
    {
        return userRepo.GetMany().ToDictionary(u => u.Id, u => u.UserName);
    }

    private static PostDto ToPostDto(Post p, Dictionary<int, string> userNames)
    {
        return new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Body = p.Body,
            UserId = p.UserId,
            AuthorUserName = userNames.GetValueOrDefault(p.UserId, "Unknown")
        };
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