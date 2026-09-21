using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public void Show()
    {
        ShowPosts(postRepository.GetMany());
    }

    public void ShowByUser(int userId)
    {
        ShowPosts(postRepository.GetMany().Where(p => p.UserId == userId));
    }

    private void ShowPosts(IQueryable<Post> posts)
    {
        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }
}