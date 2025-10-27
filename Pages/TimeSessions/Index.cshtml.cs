using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1StudentTracker.Models;
using SE1StudentTracker.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SE1StudentTracker.Pages.TimeSessions
{
    public class IndexModel : PageModel
    {
        private readonly TimeSessionRepository _repo;
        public IndexModel(TimeSessionRepository repo) => _repo = repo;

        public List<TimeSessionFull> Sessions { get; private set; } = new();

        public async Task OnGetAsync()
        {
            Sessions = await _repo.GetRecentSessionsAsync(100);
        }
    }
}
