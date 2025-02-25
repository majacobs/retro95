using Retro95.Models.Db;

namespace Retro95.Models;

public class TaskBar
{
    public required string ApplicationName { get; init; }
    public required ICollection<Team> Teams { get; init; }
    public Guid? SessionId { get; init; }
}