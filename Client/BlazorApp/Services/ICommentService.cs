using DTOs;

namespace BlazorApp.Services;

public interface ICommentService
{
    public Task<List<CommentDto>> GetCommentsAsync(int? userId = null, string? userName = null, int? postId = null);
    public Task<CommentDto> GetCommentAsync(int id);
    public Task UpdateCommentAsync(int id, UpdateCommentDto request);
    public Task DeleteCommentAsync(int id);
}