using Helpdesk_System.Data;
using Helpdesk_System.Models.Entities;
using Helpdesk_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_System.Services
{
    public class TicketService : ITicketService
    {
        private readonly HelpdeskSystemDbContext _context;

        public TicketService(HelpdeskSystemDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllAsync(int? statusId = null, int? priorityId = null, int? agentId = null)
        {
            var query = _context.Tickets
                .Include(t => t.Requestor)
                .Include(t => t.Agent)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .AsQueryable();

            if (statusId.HasValue)
                query = query.Where(t => t.StatusId == statusId.Value);

            if (priorityId.HasValue)
                query = query.Where(t => t.PriorityId == priorityId.Value);

            if (agentId.HasValue)
                query = query.Where(t => t.AgentId == agentId.Value);

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Requestor)
                .Include(t => t.Agent)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .Include(t => t.Department)

                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)

                .Include(t => t.History)
                    .ThenInclude(h => h.Status)

                .Include(t => t.History)
                    .ThenInclude(h => h.Agent)

                .Include(t => t.History)
                    .ThenInclude(h => h.ChangedByUser)

                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateAsync(Ticket ticket)
        {
            ticket.CreatedAt = DateTime.Now;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await AddHistoryAsync(ticket, ticket.RequestorId);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            ticket.UpdatedAt = DateTime.Now;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            await AddHistoryAsync(ticket, ticket.RequestorId);
        }

        public async Task UpdateAsync(Ticket ticket, int? changedBy)
        {
            ticket.UpdatedAt = DateTime.Now;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            await AddHistoryAsync(ticket, changedBy);
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
                return;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        private async Task AddHistoryAsync(Ticket ticket, int? changedBy)
        {
            var history = new TicketHistory
            {
                TicketId = ticket.Id,
                RequestorId = ticket.RequestorId,
                DepartmentId = ticket.DepartmentId,
                AgentId = ticket.AgentId,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                StatusId = ticket.StatusId,
                PriorityId = ticket.PriorityId,
                CreatedAt = ticket.CreatedAt,
                AssignedAt = ticket.AssignedAt,
                UpdatedAt = ticket.UpdatedAt,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                ChangedBy = changedBy,
                HistoryCreatedAt = DateTime.Now
            };

            _context.TicketsHistory.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}