using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public void Show()
    {
        ShowUsers(userRepository.GetMany());
    }

    public void ShowBySearch(string search)
    {
        IQueryable<User> matches = userRepository.GetMany()
            .Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase));
        ShowUsers(matches);
    }

    private void ShowUsers(IQueryable<User> users)
    {
        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (User user in users)
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }
}