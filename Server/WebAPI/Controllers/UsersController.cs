using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;


[ApiController]
[Route("users")]

public class UsersController:ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany([FromQuery] string? userName)
    {
        IQueryable<User> users = userRepo.GetMany();
        
        if (!string.IsNullOrWhiteSpace(userName))
            users = users.Where(u => u.UserName.ToLower().Contains(userName.ToLower()));

        List<UserDto> result = users
            .Select(u => new UserDto { Id = u.Id, UserName = u.UserName })
            .ToList();
        
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);
            return Ok(new UserDto { Id = user.Id, UserName = user.UserName });
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
       
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto userDto)
    {
        bool taken = userRepo.GetMany()
            .Any(u => u.UserName.ToLower() == userDto.UserName.ToLower());
        if (taken)
            return Conflict($"Username '{userDto.UserName}' is already taken");  

        User user = new User { UserName = userDto.UserName, Password = userDto.Password };
        User created = await userRepo.AddAsync(user);

        UserDto dto = new UserDto { Id = created.Id, UserName = created.UserName };
        return Created($"/users/{dto.Id}", dto);  

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);

            if (user.UserName.ToLower() != userDto.UserName.ToLower())
            {
                bool taken = userRepo.GetMany()
                    .Any(u => u.UserName.ToLower() == userDto.UserName.ToLower());
                if (taken)
                    return Conflict($"Username '{userDto.UserName}' is already taken");
            }

            user.UserName = userDto.UserName;
            user.Password = userDto.Password;
            await userRepo.UpdateAsync(user);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
    }
}