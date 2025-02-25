using Microsoft.AspNetCore.Mvc;
using Retro95.Models.Db;

namespace Retro95.Pages;

public class IndexModel(RetroContext context) : BasePageModel(context)
{
    private readonly RetroContext _context = context;
    
    public async Task<IActionResult> OnGetAsync()
    {
        await PopulateStartMenu();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateTeamAsync(string name)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest();
        }

        var entry = _context.Teams.Add(new Team
        {
            Name = name,
        });

        await _context.SaveChangesAsync();

        return RedirectToPage("Team", new { teamId = entry.Entity.Id });
    }
}