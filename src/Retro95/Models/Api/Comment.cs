namespace Retro95.Models.Api;

public class Comment
{
    public Guid SessionId { get; set; }
    
    public required string Text { get; set; }
    
    public required string Type { get; set; }
    
    public required string AuthorName { get; set; }
}