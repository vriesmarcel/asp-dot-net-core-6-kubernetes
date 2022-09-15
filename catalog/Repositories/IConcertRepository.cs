using GloboTicket.Catalog.Model;

namespace GloboTicket.Catalog.Repositories;

public interface IConcertRepository
{
  IEnumerable<Concert> GetConcerts();
  Task<Concert> GetConcertById(Guid concertId);
}
