using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentsView
{
    private readonly ICommentRepository commentRepository;

    public ListCommentsView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public void ShowByUser(int userId)
    {
        List<Comment> comments = commentRepository.GetMany()
            .Where(c => c.UserId == userId)
            .ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("No comments found for this user.");
            return;
        }

        foreach (Comment comment in comments)
        {
            Console.WriteLine($"[{comment.Id}] (post {comment.PostId}): {comment.Body}");
        }
    }
}