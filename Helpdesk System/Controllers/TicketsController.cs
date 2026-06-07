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
        private const int ClosedStatusId = 7;

        public TicketsController(ITicketService ticketService, HelpdeskSystemDbContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        public async Task<IActionResult> Index(string view = "active", int? statusId = null, int? priorityId = null, int? agentId = null)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var currentUserId = int.Parse(userIdClaim);
            var roleName = User.FindFirstValue(ClaimTypes.Role);

            if (view == "new")
            {
                statusId = 1;
            }
            else if (view == "closed")
            {
                statusId = 7;
            }

            var tickets = await _ticketService.GetAllAsync(
                statusId,
                priorityId,
                agentId,
                currentUserId,
                roleName);

            if (view == "active")
            {
                tickets = tickets
                    .Where(t => t.StatusId != 1 && t.StatusId != 7)
                    .ToList();
            }

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
            ViewBag.CurrentView = view;

            return View(tickets);
        }
        public async Task<IActionResult> Dashboard()
        {
            var model = new TicketDashboardViewModel
            {
                TotalTickets = await _context.Tickets.CountAsync(),
                NewTickets = await _context.Tickets.CountAsync(t => t.StatusId == 1),
                InProgressTickets = await _context.Tickets.CountAsync(t => t.StatusId == 2),
                ResolvedTickets = await _context.Tickets.CountAsync(t => t.StatusId == 6),
                ClosedTickets = await _context.Tickets.CountAsync(t => t.StatusId == 7),
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {

            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var currentUserId = int.Parse(userIdClaim);

            if (User.IsInRole("Requestor") &&
                ticket.RequestorId != currentUserId)
            {
                return Forbid();
            }

            if (User.IsInRole("Agent") &&
                ticket.AgentId != currentUserId &&
                ticket.AgentId != null)
            {
                return Forbid();
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


            ViewBag.Agents = agents;

            return View(ticket);
        }

        [HttpGet]
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

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId == ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
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

            if (ticket.StatusId == 3 && User.IsInRole("Requestor"))
            {
                ticket.StatusId = 2;
                ticket.UpdatedAt = DateTime.Now;

                _context.TicketsHistory.Add(new TicketHistory
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
                    ChangedBy = int.Parse(userIdClaim),
                    HistoryCreatedAt = DateTime.Now
                });
            }

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartWork(int ticketId)
        {
            if (!CanManageTickets())
            {
                return Forbid();
            }

            return await SetTicketStatus(ticketId, 2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResumeTicket(int ticketId)
        {
            if (!CanManageTickets())
            {
                return Forbid();
            }

            return await SetTicketStatus(ticketId, 2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestCustomerResponse(int ticketId, string reason)
        {
            if (!CanManageTickets())
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId == ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var now = DateTime.Now;
            var userId = int.Parse(userIdClaim);

            ticket.StatusId = 3;
            ticket.UpdatedAt = now;
            ticket.ResolvedAt = null;

            _context.Comments.Add(new Comment
            {
                TicketId = ticket.Id,
                UserId = userId,
                Content = $"Poproszono zgłaszającego o odpowiedź. Powód: {reason}",
                IsInternal = false,
                CreatedAt = now
            });

            _context.TicketsHistory.Add(new TicketHistory
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
                ChangedBy = userId,
                HistoryCreatedAt = now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspendTicket(int ticketId, string reason)
        {
            if (!CanManageTickets())
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId == ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var now = DateTime.Now;
            var userId = int.Parse(userIdClaim);

            ticket.StatusId = 5;
            ticket.UpdatedAt = now;
            ticket.ResolvedAt = null;

            _context.Comments.Add(new Comment
            {
                TicketId = ticket.Id,
                UserId = userId,
                Content = $"Zgłoszenie zostało wstrzymane. Powód: {reason}",
                IsInternal = false,
                CreatedAt = now
            });

            _context.TicketsHistory.Add(new TicketHistory
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
                ChangedBy = userId,
                HistoryCreatedAt = now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReopenTicket(int ticketId)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId != ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var now = DateTime.Now;
            var userId = int.Parse(userIdClaim);

            ticket.StatusId = 2;
            ticket.ClosedAt = null;
            ticket.ResolvedAt = null;
            ticket.UpdatedAt = now;

            _context.Comments.Add(new Comment
            {
                TicketId = ticket.Id,
                UserId = userId,
                Content = "Zgłoszenie zostało ponownie otwarte.",
                IsInternal = false,
                CreatedAt = now
            });

            _context.TicketsHistory.Add(new TicketHistory
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
                ChangedBy = userId,
                HistoryCreatedAt = now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondToTeam(int ticketId, string content)
        {
            if (!User.IsInRole("Requestor") && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId != 3)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var now = DateTime.Now;
            var userId = int.Parse(userIdClaim);

            if (User.IsInRole("Requestor") && ticket.RequestorId != userId)
            {
                return Forbid();
            }

            ticket.StatusId = 2;
            ticket.UpdatedAt = now;
            ticket.ResolvedAt = null;

            _context.Comments.Add(new Comment
            {
                TicketId = ticket.Id,
                UserId = userId,
                Content = $"Odpowiedź dla zespołu: {content}",
                IsInternal = false,
                CreatedAt = now
            });

            _context.TicketsHistory.Add(new TicketHistory
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
                ChangedBy = userId,
                HistoryCreatedAt = now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        private async Task<IActionResult> SetTicketStatus(int ticketId, int statusId)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId == ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            if (ticket.StatusId == statusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ticket.StatusId = statusId;
            ticket.UpdatedAt = DateTime.Now;

            if (statusId == 6)
            {
                ticket.ResolvedAt = DateTime.Now;
            }
            else
            {
                ticket.ResolvedAt = null;
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(int ticketId, string closeComment)
        {
            if (!CanManageTickets())
            {
                return Forbid();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId == ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            if (string.IsNullOrWhiteSpace(closeComment))
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var now = DateTime.Now;
            var userId = int.Parse(userIdClaim);

            ticket.StatusId = ClosedStatusId;
            ticket.ClosedAt = now;
            ticket.UpdatedAt = now;

            if (ticket.ResolvedAt == null)
            {
                ticket.ResolvedAt = now;
            }

            var comment = new Comment
            {
                TicketId = ticket.Id,
                UserId = userId,
                Content = $"Zgłoszenie zostało zamknięte. Komentarz zamknięcia: {closeComment}",
                IsInternal = false,
                CreatedAt = now
            };

            _context.Comments.Add(comment);

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
                ChangedBy = userId,
                HistoryCreatedAt = now
            };

            _context.TicketsHistory.Add(history);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignAgent(int ticketId, int? agentId)
        {
            if (!CanManageTickets())
            {
                return Forbid();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ticket.StatusId == ClosedStatusId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var currentUserId = int.Parse(userIdClaim);

            if (User.IsInRole("Agent"))
            {
                agentId = currentUserId;
            }

            if (ticket.AgentId == agentId)
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            ticket.AgentId = agentId;
            ticket.AssignedAt = agentId.HasValue ? DateTime.Now : null;
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
                ChangedBy = currentUserId,
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
        private bool CanManageTickets()
        {
            return User.IsInRole("Admin") || User.IsInRole("Agent");
        }
    }

}