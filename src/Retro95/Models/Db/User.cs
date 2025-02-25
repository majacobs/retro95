namespace Retro95.Models.Db;

public class User
{
    public Guid Id { get; init; }
    public required string Name { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = [];
    public virtual ICollection<Comment> Comments { get; set; } = [];
}