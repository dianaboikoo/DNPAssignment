using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public ManagePostsView(IPostRepository postRepository, IUserRepository userRepository,
        ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync()
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("1) Create new post");
            Console.WriteLine("2) View posts overview");
            Console.WriteLine("3) View specific post");
            Console.WriteLine("4) Update post");
            Console.WriteLine("5) Delete post");
            Console.WriteLine("6) View posts by user ID");
            Console.WriteLine("0) Back");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreatePostView createPostView = new CreatePostView(postRepository, userRepository);
                    await createPostView.ShowAsync();
                    break;
                case "2":
                    ListPostsView listPostsView = new ListPostsView(postRepository);
                    listPostsView.Show();
                    break;
                case "3":
                    SinglePostView singlePostView = new SinglePostView(postRepository, commentRepository);
                    await singlePostView.ShowAsync();
                    break;
                case "4":
                    await UpdatePostAsync();
                    break;
                case "5":
                    await DeletePostAsync();
                    break;
                case "6":
                    Console.Write("Enter user ID: ");
                    if (int.TryParse(Console.ReadLine(), out int userId))
                    {
                        new ListPostsView(postRepository).ShowByUser(userId);
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
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

    private async Task UpdatePostAsync()
    {
        Console.Write("Enter ID of post to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Post existing;
        try
        {
            existing = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        Console.Write($"Enter new title (leave empty to keep '{existing.Title}'): ");
        string title = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(title))
        {
            existing.Title = title;
        }

        Console.Write("Enter new body (leave empty to keep current): ");
        string body = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(body))
        {
            existing.Body = body;
        }

        await postRepository.UpdateAsync(existing);
        Console.WriteLine("Post updated.");
    }

    private async Task DeletePostAsync()
    {
        Console.Write("Enter ID of post to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            await postRepository.DeleteAsync(id);
            Console.WriteLine("Post deleted.");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}