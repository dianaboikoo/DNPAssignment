using DTOs;

namespace BlazorApp.Services;

public interface IUserService
{
    public Task<UserDto> AddUserAsync(CreateUserDto request);
    public Task<UserDto> GetUserAsync(int id);
    public Task<List<UserDto>> GetUsersAsync(string? userName = null);
    public Task UpdateUserAsync(int id, UpdateUserDto request);
    public Task DeleteUserAsync(int id);
}