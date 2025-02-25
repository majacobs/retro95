using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retro95.Models.Db;

namespace Retro95.Pages;

public class TeamModel(RetroContext context) : BasePageModel(context)
{
    private readonly RetroContext _context = context;

    public Team Team { get; set; } = null!;
    public SessionInfo[] Sessions { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync(Guid teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Sessions)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == teamId);
        
        if (team is null)
        {
            return NotFound();
        }
        
        Team = team;
        Sessions = await _context.Sessions.Where(s => s.TeamId == teamId)
            .Select(s => new SessionInfo
            {
                Session = s,
                CommentCount = s.Comments.Count,
            })
            .OrderByDescending(x => x.Session.CreatedAt)
            .AsNoTracking()
            .ToArrayAsync();

        await PopulateStartMenu();
        await RecordTeamMembership(teamId);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateSessionAsync(Guid teamId, string name)
    {
        name = name.Trim();
        var team = await _context.Teams.FirstOrDefaultAsync(s => s.Id == teamId);
        if (team is null || !ModelState.IsValid || string.IsNullOrWhiteSpace(name))
        {
            return BadRequest();
        }

        var entry = _context.Sessions.Add(new Session
        {
            Name = name,
            Types = team.DefaultTypes
                .Select(t => new CommentType { Name = t.Name })
                .ToArray(),
            CreatedAt = DateTime.UtcNow,
            Team = team,
        });
        
        await _context.SaveChangesAsync();

        return RedirectToPage("Session", new { sessionId = entry.Entity.Id });
    }

    public async Task<IActionResult> OnPostUpdateDefaultsAsync(Guid teamId, string columns)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(s => s.Id == teamId);
        if (team is null || !ModelState.IsValid)
        {
            return BadRequest();
        }

        var types = ParseCommentTypes(columns);
        if (types.Length == 0)
        {
            return BadRequest();
        }

        team.DefaultTypes = types;
        await _context.SaveChangesAsync();

        return RedirectToPage("Team", new { teamId = team.Id });
    }

    private static CommentType[] ParseCommentTypes(string str) =>
        str
            .Split("\n")
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => new CommentType { Name = s })
            .ToArray();

    public class SessionInfo
    {
        public required Session Session { get; init; }
        public required int CommentCount { get; init; }
    }
}