using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository,
        IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task StartAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("1) Manage users");
            Console.WriteLine("2) Manage posts");
            Console.WriteLine("3) Manage comments");
            Console.WriteLine("0) Exit");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ManageUsersView manageUsersView = new ManageUsersView(userRepository);
                    await manageUsersView.ShowAsync();
                    break;
                case "2":
                    ManagePostsView managePostsView =
                        new ManagePostsView(postRepository, userRepository, commentRepository);
                    await managePostsView.ShowAsync();
                    break;
                case "3":
                    ManageCommentsView manageCommentsView =
                        new ManageCommentsView(commentRepository, postRepository, userRepository);
                    await manageCommentsView.ShowAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option, try again.");
                    break;
            }
        }
    }
}