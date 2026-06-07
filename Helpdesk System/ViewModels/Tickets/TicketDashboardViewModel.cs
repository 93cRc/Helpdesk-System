namespace Helpdesk_System.ViewModels.Tickets
{
    public class TicketDashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int NewTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
    }
}