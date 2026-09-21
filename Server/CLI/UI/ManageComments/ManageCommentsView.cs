using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ManageCommentsView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public ManageCommentsView(ICommentRepository commentRepository, IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("1) Add comment to a post");
            Console.WriteLine("2) View comments by a user");
            Console.WriteLine("3) Delete comment");
            Console.WriteLine("0) Back");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateCommentView createCommentView =
                        new CreateCommentView(commentRepository, postRepository, userRepository);
                    await createCommentView.ShowAsync();
                    break;
                case "2":
                    Console.Write("Enter user ID: ");
                    if (int.TryParse(Console.ReadLine(), out int userId))
                    {
                        new ListCommentsView(commentRepository).ShowByUser(userId);
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                    break;
                case "3":
                    await DeleteCommentAsync();
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

    private async Task DeleteCommentAsync()
    {
        Console.Write("Enter ID of comment to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await commentRepository.DeleteAsync(id);
            Console.WriteLine("Comment deleted.");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}