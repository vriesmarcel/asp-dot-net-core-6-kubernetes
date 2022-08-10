namespace GloboTicket.Frontend.Models.Api;

public class BasketLine
{
    public Guid BasketLineId { get; set; }
    public Guid BasketId { get; set; }
    public Guid ConcertId { get; set; }
    public int TicketAmount { get; set; }
    public int Price { get; set; }
    public Concert Concert { get; set; } = new Concert();
}
