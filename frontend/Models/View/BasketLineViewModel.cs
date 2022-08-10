namespace GloboTicket.Frontend.Models.View;

public class BasketLineViewModel
{
    public Guid LineId { get; set; }
    public Guid ConcertId { get; set; }
    public string ConcertName { get; set; } = String.Empty;
    public DateTimeOffset Date { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
}
