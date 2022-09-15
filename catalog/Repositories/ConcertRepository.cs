using GloboTicket.Catalog.Model;
using GloboTicket.Services.EventCatalog.DbContexts;
using Microsoft.Extensions.Options;

namespace GloboTicket.Catalog.Repositories;

public class ConcertRepository : IConcertRepository
{
    private readonly EventCatalogDbContext _eventCatalogDbContext;


    private readonly ILogger<ConcertRepository> logger;

    public ConcertRepository(EventCatalogDbContext eventCatalogDbContext, 
        ILogger<ConcertRepository> logger)
    {
         this.logger = logger;
        _eventCatalogDbContext = eventCatalogDbContext;
    }

     public IEnumerable<Concert> GetConcerts()
    {
            return _eventCatalogDbContext.Concerts;
    }

    public Task<Concert> GetConcertById(Guid concertId)
    {
        var concert = _eventCatalogDbContext.Concerts.FirstOrDefault(e => e.ConcertId == concertId);
        if (concert == null)
        {
            throw new InvalidOperationException("Concert not found");
        }
        return Task.FromResult(concert);
    }
}
