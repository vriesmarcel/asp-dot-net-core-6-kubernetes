using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Frontend.Models.Api;

public class BasketLineForCreation
{
    [Required]
    public Guid ConcertId { get; set; }
    [Required]
    public int TicketAmount { get; set; }
    [Required]
    public int Price { get; set; }
}
