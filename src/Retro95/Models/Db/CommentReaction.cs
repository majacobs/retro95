namespace Retro95.Models.Db;

public class CommentReaction
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public required string Image { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual Comment Comment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}