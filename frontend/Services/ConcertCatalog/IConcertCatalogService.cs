using GloboTicket.Frontend.Models.Api;

namespace GloboTicket.Frontend.Services;

public interface IConcertCatalogService
{
    Task<IEnumerable<Concert>> GetAll();

    Task<Concert> GetConcert(Guid id);

}
