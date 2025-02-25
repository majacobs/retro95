namespace Retro95.Models.Db;

public class Team
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public ICollection<CommentType> DefaultTypes { get; set; } = [];

    public virtual ICollection<Session> Sessions { get; set; } = [];
    public virtual ICollection<User> Users { get; set; } = [];
}