namespace DTOs;

public class CreateUserDto
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}

public class UpdateUserDto
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
}
