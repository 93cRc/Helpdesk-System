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

        /*
        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Requestor)
                .Include(t => t.Agent)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .ToListAsync();
        }
        */

        public async Task<List<Ticket>> GetAllAsync() //TEST
        {
            var count = await _context.Tickets.CountAsync();

            Console.WriteLine($"LICZBA TICKETÓW: {count}");

            return await _context.Tickets
                .Include(t => t.Requestor)
                .Include(t => t.Agent)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
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
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateAsync(Ticket ticket)
        {
            ticket.CreatedAt = DateTime.Now;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            ticket.UpdatedAt = DateTime.Now;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
                return;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }
}