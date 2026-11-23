using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(title))
            posts = posts.Where(p => p.Title.ToLower().Contains(title.ToLower()));

        if (userId != null)
            posts = posts.Where(p => p.UserId == userId);

        if (!string.IsNullOrWhiteSpace(userName))
            posts = posts.Where(p => p.User.UserName.ToLower().Contains(userName.ToLower()));

        List<PostDto> result = await posts
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId,
                AuthorUserName = p.User.UserName
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost(int id, [FromQuery] bool includeComments = false)
    {
        PostDto? dto = await postRepo.GetMany()
            .Where(p => p.Id == id)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId,
                AuthorUserName = p.User.UserName
            })
            .FirstOrDefaultAsync();

        if (dto == null)
            return NotFound($"Post with id {id} not found");

        if (includeComments)
        {
            dto.Comments = await commentRepo.GetMany()
                .Where(c => c.PostId == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserId = c.UserId,
                    PostId = c.PostId,
                    AuthorUserName = c.User.UserName
                })
                .ToListAsync();
        }

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto postDto)
    {
        User? author = await userRepo.GetMany().FirstOrDefaultAsync(u => u.Id == postDto.UserId);
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
        bool postExists = await postRepo.GetMany().AnyAsync(p => p.Id == id);
        if (!postExists)
            return NotFound($"Post with id {id} not found");

        User? author = await userRepo.GetMany().FirstOrDefaultAsync(u => u.Id == commentDto.UserId);
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
        bool postExists = await postRepo.GetMany().AnyAsync(p => p.Id == id);
        if (!postExists)
            return NotFound($"Post with id {id} not found");

        List<CommentDto> result = await commentRepo.GetMany()
            .Where(c => c.PostId == id)
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
}