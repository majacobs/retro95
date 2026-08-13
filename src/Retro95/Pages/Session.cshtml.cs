using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retro95.Models;
using Retro95.Models.Db;

namespace Retro95.Pages;

public class SessionModel(RetroContext context) : BasePageModel(context)
{
    private readonly RetroContext _context = context;

    public Session Session { get; set; } = null!;
    public new User? User { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid sessionId)
    {
        var session = await _context.Sessions
            .Include(s => s.Team)
            .Include(s => s.Comments)
            .ThenInclude(c => c.User)
            .Include(s => s.Comments)
            .ThenInclude(c => c.Reactions)
            .ThenInclude(r => r.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            return NotFound();
        }

        var userId = GetUserId();

        Session = session;
        User = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        await PopulateStartMenu();
        await RecordTeamMembership(session.TeamId);
        return Page();
    }

    public async Task<IActionResult> OnGetCsvAsync(Guid sessionId)
    {
        var session = await _context.Sessions
            .Include(s => s.Comments)
            .ThenInclude(c => c.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            return NotFound();
        }

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8);

        await writer.WriteLineAsync("\"Column\",\"Author\",\"Text\"");
        foreach (var comment in session.Comments.OrderBy(c => c.CreatedAt))
        {
            writer.WriteLine("\"{0}\",\"{1}\",\"{2}\"",
                CsvEncode(comment.Type),
                CsvEncode(comment.User.Name),
                CsvEncode(comment.Text));
        }

        await writer.FlushAsync();
        stream.Seek(0, SeekOrigin.Begin);

        return File(stream, "text/csv", $"{session.Name}.csv");

        static string CsvEncode(string s) => s.Replace("\"", "\"\"");
    }

    public async Task<IActionResult> OnPostAddCommentAsync(Guid sessionId, Models.Api.Comment newComment)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = GetUserId(setIfNull: true)!.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            user = new User { Id = userId, Name = newComment.AuthorName };
        }
        else if (user.Name != newComment.AuthorName)
        {
            user.Name = newComment.AuthorName;
        }

        _context.Comments.Add(new Comment
        {
            Session = session,
            Text = newComment.Text.Trim(),
            Type = newComment.Type,
            RenderAs = newComment.RenderAs,
            User = user,
            CreatedAt = DateTime.UtcNow,
        });

        await _context.SaveChangesAsync();


        return RedirectToPage("Session", new { sessionId = session.Id });
    }

    public async Task<IActionResult> OnPostRenameAsync(Guid sessionId, string name)
    {
        name = name.Trim();
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null || !ModelState.IsValid || string.IsNullOrWhiteSpace(name))
        {
            return BadRequest();
        }

        session.Name = name;
        await _context.SaveChangesAsync();

        return RedirectToPage("Session", new { sessionId = session.Id });
    }

    public async Task<IActionResult> OnPostAddColumnAsync(Guid sessionId, string name)
    {
        name = name.Trim();
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null || !ModelState.IsValid || session.Types.Any(t => t.Name == name))
        {
            return BadRequest();
        }

        session.Types.Add(new CommentType { Name = name });
        await _context.SaveChangesAsync();

        return RedirectToPage("Session", new { sessionId = session.Id });
    }

    public async Task<IActionResult> OnPostRemoveColumnAsync(Guid sessionId, string name)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session is null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        var type = session.Types.FirstOrDefault(t => t.Name == name);
        if (type is null)
        {
            return BadRequest();
        }

        session.Types.Remove(type);
        await _context.SaveChangesAsync();

        await _context.Comments
            .Where(c => c.SessionId == sessionId && c.Type == type.Name)
            .ExecuteDeleteAsync();

        return RedirectToPage("Session", new { sessionId = session.Id });
    }

    public async Task<IActionResult> OnPostRemoveCommentAsync(Guid sessionId, Guid commentId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        var authorId = GetUserId();
        if (comment is null || comment.UserId != authorId)
        {
            return BadRequest();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage("Session", new { sessionId });
    }

    public async Task<IActionResult> OnGetNewComments(Guid sessionId, DateTime lastUpdate)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.SessionId == sessionId && c.CreatedAt > lastUpdate)
            .OrderBy(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        var reactionsWithUpdates = _context.CommentReactions
            .Where(r => r.Comment.SessionId == sessionId && r.CreatedAt > lastUpdate)
            .Select(r => new { r.CommentId, r.Image })
            .Distinct();

        var reactions = await _context.CommentReactions
            .Include(c => c.User)
            .Join(reactionsWithUpdates, r => new { r.CommentId, r.Image }, rwu => new { rwu.CommentId, rwu.Image }, (r, _) => r)
            .GroupBy(r => new { r.CommentId, r.Image })
            .AsNoTracking()
            .ToListAsync();

        return new JsonResult(new
        {
            comments = comments.Select(c => new
            {
                c.Id,
                AuthorName = c.User.Name,
                c.Text,
                c.Type,
            }),
            reactions = reactions
                .OrderBy(grouping => grouping.Min(r => r.CreatedAt))
                .Select(grouping => new
                {
                    grouping.Key.CommentId,
                    grouping.Key.Image,
                    Authors = grouping
                        .OrderBy(r => r.CreatedAt)
                        .Select(r => r.User.Name)
                        .ToList(),
                }),
        });
    }

    public async Task<IActionResult> OnPostAddReaction(Guid commentId, string reaction)
    {
        var userId = GetUserId();
        
        if (!Reaction.All.Contains(reaction) || userId is null)
        {
            return BadRequest();
        }

        _context.CommentReactions.Add(new CommentReaction
        {
            CommentId = commentId,
            UserId = userId.Value,
            Image = reaction,
            CreatedAt = DateTime.UtcNow,
        });

        await _context.SaveChangesAsync();

        var comment = await _context.Comments.FirstAsync(c => c.Id == commentId);
        
        return RedirectToPage("Session", new { sessionId = comment.SessionId });
    }

    public async Task<IActionResult> OnPostToggleReaction(Guid commentId, string reaction)
    {
        var userId = GetUserId();
        
        if (!Reaction.All.Contains(reaction) || userId is null)
        {
            return BadRequest();
        }

        var deleted = await _context.CommentReactions
            .Where(c => c.CommentId == commentId
                        && c.UserId == userId.Value
                        && c.Image == reaction)
            .ExecuteDeleteAsync();
        
        if (deleted == 0)
        {
            _context.CommentReactions.Add(new CommentReaction
            {
                CommentId = commentId,
                UserId = userId.Value,
                Image = reaction,
                CreatedAt = DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();
        }

        var comment = await _context.Comments.FirstAsync(c => c.Id == commentId);
        
        return RedirectToPage("Session", new { sessionId = comment.SessionId });
    }
}
