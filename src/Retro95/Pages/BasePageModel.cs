using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Retro95.Models.Db;

namespace Retro95.Pages;

public abstract class BasePageModel(RetroContext context) : PageModel
{
    private const string UserIdCookie = "UserId";

    public ICollection<Team> Teams { get; set; } = [];

    protected async Task PopulateStartMenu()
    {
        var userId = GetUserId();
        
        var user = await context.Users
            .Include(user => user.Teams)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        Teams = user?.Teams ?? [];
    }

    protected async Task RecordTeamMembership(Guid teamId)
    {
        var userId = GetUserId(setIfNull: true)!.Value;
        var user = await context.Users.Include(u => u.Teams).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            user = new User
            {
                Id = userId,
                Name = string.Empty,
            };
            context.Users.Add(user);
        }

        if (user.Teams.Any(t => t.Id == teamId))
        {
            return;
        }
        
        var team = await context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
        team?.Users.Add(user);

        await context.SaveChangesAsync();
    }
    
    protected Guid? GetUserId(bool setIfNull = false)
    {
        if (Request.Cookies.TryGetValue(UserIdCookie, out var authorIdString)
            && Guid.TryParse(authorIdString, out var authorId))
        {
            return authorId;
        }

        if (!setIfNull)
        {
            return null;
        }
        
        authorId = Guid.NewGuid();
        Response.Cookies.Append(UserIdCookie, authorId.ToString(),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(100) });
        return authorId;
    }
}