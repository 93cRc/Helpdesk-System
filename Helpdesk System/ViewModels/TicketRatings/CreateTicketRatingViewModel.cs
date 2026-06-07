using System.ComponentModel.DataAnnotations;

namespace Helpdesk_System.ViewModels.TicketRatings {
	public class CreateTicketRatingViewModel {
		public int TicketId { get; set; }

		public string TicketTitle { get; set; } = string.Empty;

		[Required(ErrorMessage = "Wybierz ocenę.")]
		[Range(1, 5, ErrorMessage = "Ocena musi mieścić się w zakresie od 1 do 5.")]
		[Display(Name = "Ocena")]
		public byte Rating { get; set; }

		[StringLength(1000, ErrorMessage = "Komentarz może mieć maksymalnie 1000 znaków.")]
		[Display(Name = "Komentarz")]
		public string? Content { get; set; }
	}
}