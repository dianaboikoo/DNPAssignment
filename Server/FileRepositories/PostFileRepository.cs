using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        List<Post> posts = await LoadPostsAsync();
        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;
        post.Id = maxId + 1;
        posts.Add(post);
        await SavePostsAsync(posts);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await LoadPostsAsync();
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post '{post.Id}' was not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);
        await SavePostsAsync(posts);
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await LoadPostsAsync();
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post'{id}' was not found");
        }

        posts.Remove(postToRemove);
        await SavePostsAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await LoadPostsAsync();
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post'{id}' was not found");
        }

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string postsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts.AsQueryable();
    }

    private async Task<List<Post>> LoadPostsAsync()
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
    }

    private async Task SavePostsAsync(List<Post> posts)
    {
        string postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }
}