namespace Retro95.Models.Db;

public class Session
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string? Name { get; set; }
    public ICollection<CommentType> Types { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    public virtual Team Team { get; set; } = null!;
    public virtual ICollection<Comment> Comments { get; set; } = [];
}