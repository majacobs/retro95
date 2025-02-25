namespace Retro95.Models.Db;

public class Comment
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public required string Text { get; set; }
    public required string Type { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Session Session { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}