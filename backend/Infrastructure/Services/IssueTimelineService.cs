using backend.Infrastructure;
using backend.Models;
using backend.Enums;

namespace backend.Infrastructure.Services
{
    public class IssueTimelineService
    {
        private readonly AppDbContext _db;

        public IssueTimelineService(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddTimelineEntryAsync(
            long issueId,
            string action,
            string performedBy,
            string? notes = null)
        {
            if (!Enum.TryParse<TimelineAction>(action, true, out var timelineAction))
            {
                throw new ArgumentException($"Invalid timeline action: {action}");
            }

            var entry = new IssueTimeline
            {
                IssueId = issueId,
                Action = timelineAction,
                PerformedBy = performedBy,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            _db.IssueTimelines.Add(entry);

            await _db.SaveChangesAsync();
        }
    }
}
