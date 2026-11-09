using System.Net.Http.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("posts", request);
        return await response.ReadAsAsync<PostDto>();
    }

    public async Task<List<PostDto>> GetPostsAsync(string? title = null, int? userId = null, string? userName = null)
    {
        List<string> filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(title))
            filters.Add($"title={Uri.EscapeDataString(title)}");
        if (userId != null)
            filters.Add($"userId={userId}");
        if (!string.IsNullOrWhiteSpace(userName))
            filters.Add($"userName={Uri.EscapeDataString(userName)}");

        string url = "posts";
        if (filters.Count > 0)
            url += "?" + string.Join("&", filters);

        HttpResponseMessage response = await client.GetAsync(url);
        return await response.ReadAsAsync<List<PostDto>>();
    }

    public async Task<PostDto> GetPostAsync(int id, bool includeComments = false)
    {
        HttpResponseMessage response = await client.GetAsync($"posts/{id}?includeComments={includeComments}");
        return await response.ReadAsAsync<PostDto>();
    }

    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"posts/{id}", request);
        await response.EnsureSuccessAsync();
    }

    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"posts/{id}");
        await response.EnsureSuccessAsync();
    }

    public async Task<CommentDto> AddCommentAsync(int postId, CreateCommentDto request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync($"posts/{postId}/comments", request);
        return await response.ReadAsAsync<CommentDto>();
    }

    public async Task<List<CommentDto>> GetCommentsOfPostAsync(int postId)
    {
        HttpResponseMessage response = await client.GetAsync($"posts/{postId}/comments");
        return await response.ReadAsAsync<List<CommentDto>>();
    }
}