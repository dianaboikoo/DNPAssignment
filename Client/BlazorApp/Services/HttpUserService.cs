using System.Net.Http.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("users", request);
        return await response.ReadAsAsync<UserDto>();
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        HttpResponseMessage response = await client.GetAsync($"users/{id}");
        return await response.ReadAsAsync<UserDto>();
    }

    public async Task<List<UserDto>> GetUsersAsync(string? userName = null)
    {
        string url = "users";
        if (!string.IsNullOrWhiteSpace(userName))
            url += $"?userName={Uri.EscapeDataString(userName)}";

        HttpResponseMessage response = await client.GetAsync(url);
        return await response.ReadAsAsync<List<UserDto>>();
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"users/{id}", request);
        await response.EnsureSuccessAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"users/{id}");
        await response.EnsureSuccessAsync();
    }
}