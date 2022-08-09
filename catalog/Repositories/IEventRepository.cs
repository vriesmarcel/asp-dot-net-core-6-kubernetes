namespace GloboTicket.Catalog.Repositories;

public interface IEventRepository
{
  IEnumerable<Event> GetEvents();
  Task<Event> GetEventById(Guid eventId);
  void UpdateSpecialOffer();
}
