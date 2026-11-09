using System.Net.Http.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int? userId = null, string? userName = null, int? postId = null)
    {
        List<string> filters = new List<string>();
        if (userId != null)
            filters.Add($"userId={userId}");
        if (!string.IsNullOrWhiteSpace(userName))
            filters.Add($"userName={Uri.EscapeDataString(userName)}");
        if (postId != null)
            filters.Add($"postId={postId}");

        string url = "comments";
        if (filters.Count > 0)
            url += "?" + string.Join("&", filters);

        HttpResponseMessage response = await client.GetAsync(url);
        return await response.ReadAsAsync<List<CommentDto>>();
    }

    public async Task<CommentDto> GetCommentAsync(int id)
    {
        HttpResponseMessage response = await client.GetAsync($"comments/{id}");
        return await response.ReadAsAsync<CommentDto>();
    }

    public async Task UpdateCommentAsync(int id, UpdateCommentDto request)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"comments/{id}", request);
        await response.EnsureSuccessAsync();
    }

    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"comments/{id}");
        await response.EnsureSuccessAsync();
    }
}