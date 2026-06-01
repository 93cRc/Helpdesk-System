using Helpdesk_System.Data;
using Helpdesk_System.Models.Entities;
using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Helpdesk_System.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly HelpdeskSystemDbContext _context;

        public TicketsController(ITicketService ticketService, HelpdeskSystemDbContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        public async Task<IActionResult> Index(int? statusId, int? priorityId, int? agentId)
        {
            var tickets = await _ticketService.GetAllAsync(statusId, priorityId, agentId);

            ViewBag.Statuses = await _context.Statuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            ViewBag.Priorities = await _context.Priorities
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            ViewBag.Agents = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Name == "Agent" && u.IsActive)
                .OrderBy(u => u.LastName)
                .ToListAsync();

            ViewBag.SelectedStatusId = statusId;
            ViewBag.SelectedPriorityId = priorityId;
            ViewBag.SelectedAgentId = agentId;

            return View(tickets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }
            var statuses = await _context.Statuses
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

           ViewBag.Statuses = statuses;

            var agents = await _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.Name == "Agent" && u.IsActive)
            .OrderBy(u => u.LastName)
            .ToListAsync();

            // 02.06.2026 - brak użytkowników z rolą Agent w bazie testowej.
            // Funkcjonalność przypisywania została zaimplementowana i wymaga
            // jedynie dodania kont z rolą Agent.

            ViewBag.Agents = agents;

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            var model = new CreateTicketViewModel();
            await LoadSelectLists(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int ticketId, string content, bool isInternal = false)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == ticketId);

            if (!ticketExists)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                TicketId = ticketId,
                UserId = int.Parse(userIdClaim),
                Content = content,
                IsInternal = isInternal,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int ticketId, int statusId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var history = new TicketHistory
            {
                TicketId = ticket.Id,
                RequestorId = ticket.RequestorId,
                DepartmentId = ticket.DepartmentId,
                AgentId = ticket.AgentId,
                Title = ticket.Title,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                StatusId = statusId,
                PriorityId = ticket.PriorityId,
                CreatedAt = ticket.CreatedAt,
                AssignedAt = ticket.AssignedAt,
                UpdatedAt = DateTime.Now,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                ChangedBy = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim),
                HistoryCreatedAt = DateTime.Now
            };

            _context.TicketsHistory.Add(history);

            ticket.StatusId = statusId;
            ticket.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignAgent(int ticketId, int? agentId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ticket.AgentId = agentId;
            ticket.AssignedAt = DateTime.Now;
            ticket.UpdatedAt = DateTime.Now;

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
                ChangedBy = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim),
                HistoryCreatedAt = DateTime.Now
            };

            _context.TicketsHistory.Add(history);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        public async Task<IActionResult> Create(CreateTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists(model);
                return View(model);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var status = await _context.Statuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .FirstOrDefaultAsync();


            if (status == null)
            {
                ModelState.AddModelError(string.Empty, "Brak aktywnego statusu w bazie.");
                await LoadSelectLists(model);
                return View(model);
            }

            var ticket = new Ticket
            {
                RequestorId = int.Parse(userIdClaim),
                DepartmentId = model.DepartmentId,
                CategoryId = model.CategoryId,
                PriorityId = model.PriorityId,
                StatusId = status.Id,
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };

            await _ticketService.CreateAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectLists(CreateTicketViewModel model)
        {
            model.Departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToListAsync();

            model.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            model.Priorities = await _context.Priorities
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToListAsync();
        }
    }
}