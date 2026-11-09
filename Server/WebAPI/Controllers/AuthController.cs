using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        User? user = userRepo.GetMany()
            .FirstOrDefault(u => u.UserName.ToLower() == request.UserName.ToLower());

        if (user is null)
            return Unauthorized("User not found");

        if (user.Password != request.Password)
            return Unauthorized("Incorrect password");

        UserDto dto = new UserDto { Id = user.Id, UserName = user.UserName };
        return Ok(dto);
    }
}