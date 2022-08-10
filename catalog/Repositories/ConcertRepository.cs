using Microsoft.Extensions.Options;

namespace GloboTicket.Catalog.Repositories;

public class ConcertRepository : IConcertRepository
{
    private List<Concert> concerts = new List<Concert>();
    private readonly IOptions<CatalogOptions> options;
    private readonly ILogger<ConcertRepository> logger;

    public ConcertRepository(IOptions<CatalogOptions> options,
        ILogger<ConcertRepository> logger)
    {
        this.options = options;
        this.logger = logger;

        LoadSampleData();
    }

    private void LoadSampleData()
    {
        var johnEgbertGuid = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA317}");
        var nickSailorGuid = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA318}");
        var michaelJohnsonGuid = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA319}");

        concerts.Add(new Concert
        {
            ConcertId = johnEgbertGuid,
            Name = "John Egbert Live",
            Price = 65,
            Artist = "John Egbert",
            Date = DateTime.Now.AddMonths(6),
            Description = "Join John for his farwell tour across 15 continents. John really needs no introduction since he has already mesmerized the world with his banjo.",
            ImageUrl = "/img/banjo.jpg",
        });

        concerts.Add(new Concert
        {
            ConcertId = michaelJohnsonGuid,
            Name = "The State of Affairs: Michael Live!",
            Price = 85,
            Artist = "Michael Johnson",
            Date = DateTime.Now.AddMonths(9),
            Description = "Michael Johnson doesn't need an introduction. His 25 concert across the globe last year were seen by thousands. Can we add you to the list?",
            ImageUrl = "/img/michael.jpg",
        });

        concerts.Add(new Concert
        {
            ConcertId = nickSailorGuid,
            Name = "To the Moon and Back",
            Price = 135,
            Artist = "Nick Sailor",
            Date = DateTime.Now.AddMonths(8),
            Description = "The critics are over the moon and so will you after you've watched this sing and dance extravaganza written by Nick Sailor, the man from 'My dad and sister'.",
            ImageUrl = "/img/musical.jpg",
        });
    }

    public IEnumerable<Concert> GetConcerts()
    {
        try
        {
            var connectionString =  GetConnectionString();
            logger.LogInformation($"Connection string {connectionString}");
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to fetch the connection string");
        }
        return concerts;
    }

    private string GetConnectionString()
    {
        return options?.Value?.CatalogConnectionString ?? String.Empty;
    }

    public Task<Concert> GetConcertById(Guid concertId)
    {
        var concert = concerts.FirstOrDefault(e => e.ConcertId == concertId);
        if (concert == null)
        {
            throw new InvalidOperationException("Concert not found");
        }
        return Task.FromResult(concert);
    }

    // Scheduled task calls this periodically to put one item on special offer
    public void UpdateSpecialOffer()
    {
        // reset all tickets to their default
        concerts.Clear();
        LoadSampleData();

        // Pick a random one to put on special offer
        var random = new Random();
        var specialOfferConcert = concerts[random.Next(0, concerts.Count)];
        
        // 20 percent off
        specialOfferConcert.Price = (int)(specialOfferConcert.Price * 0.8);
    }
}
