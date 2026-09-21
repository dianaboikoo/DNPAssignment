using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("1) Create new user");
            Console.WriteLine("2) View all users");
            Console.WriteLine("3) Update user");
            Console.WriteLine("4) Delete user");
            Console.WriteLine("5) Search users by username");
            Console.WriteLine("0) Back");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateUserView createUserView = new CreateUserView(userRepository);
                    await createUserView.ShowAsync();
                    break;
                case "2":
                    ListUsersView listUsersView = new ListUsersView(userRepository);
                    listUsersView.Show();
                    break;
                case "3":
                    await UpdateUserAsync();
                    break;
                case "4":
                    await DeleteUserAsync();
                    break;
                case "5":
                    Console.Write("Enter part of the username to search for: ");
                    string search = Console.ReadLine() ?? "";
                    new ListUsersView(userRepository).ShowBySearch(search);
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option, try again.");
                    break;
            }
        }
    }

    private async Task UpdateUserAsync()
    {
        Console.Write("Enter ID of user to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        User existing;
        try
        {
            existing = await userRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        Console.Write($"Enter new user name (leave empty to keep '{existing.UserName}'): ");
        string userName = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(userName))
        {
            existing.UserName = userName;
        }

        Console.Write("Enter new password (leave empty to keep current): ");
        string password = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(password))
        {
            existing.Password = password;
        }

        await userRepository.UpdateAsync(existing);
        Console.WriteLine("User updated.");
    }

    private async Task DeleteUserAsync()
    {
        Console.Write("Enter ID of user to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine("User deleted.");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}