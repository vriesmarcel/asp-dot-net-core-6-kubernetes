using GloboTicket.Frontend.Models.Api;

namespace GloboTicket.Frontend.Models.View;

public class ConcertListModel
{
    public IEnumerable<Concert> Concerts { get; set; } = new List<Concert>();
    public int NumberOfItems { get; set; }
}
