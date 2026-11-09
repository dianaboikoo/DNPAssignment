using DTOs;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> CreatePostAsync(CreatePostDto request);
    public Task<List<PostDto>> GetPostsAsync(string? title = null, int? userId = null, string? userName = null);
    public Task<PostDto> GetPostAsync(int id, bool includeComments = false);
    public Task UpdatePostAsync(int id, UpdatePostDto request);
    public Task DeletePostAsync(int id);

    public Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto request);
    public Task<List<CommentDto>> GetCommentsOfPostAsync(int postId);
}