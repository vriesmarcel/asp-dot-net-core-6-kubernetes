using GloboTicket.Frontend.Models;
using GloboTicket.Frontend.Models.Api;

#pragma warning disable 1998

namespace GloboTicket.Frontend.Services.ShoppingBasket;

// In memory implementation for demo purposes only
public class InMemoryShoppingBasketService : IShoppingBasketService
{
    private readonly Dictionary<Guid, InMemoryBasket> baskets;
    private readonly Dictionary<Guid, Concert> concertsCache; // shopping basket lines need to get concert date and name
    private readonly Settings settings;
    private readonly IConcertCatalogService concertCatalogService;

    public InMemoryShoppingBasketService(Settings settings, IConcertCatalogService concertCatalogService)
    {
        this.settings = settings;
        this.concertCatalogService = concertCatalogService;
        this.baskets = new Dictionary<Guid, InMemoryBasket>();
        this.concertsCache = new Dictionary<Guid, Concert>();
    }

    public async Task<BasketLine> AddToBasket(Guid basketId, BasketLineForCreation basketLine)
    {
        if (!baskets.TryGetValue(basketId, out var basket))
        {
            basket = new InMemoryBasket(settings.UserId);
            baskets.Add(basket.BasketId, basket);
        }
        if (!concertsCache.TryGetValue(basketLine.ConcertId, out var concert))
        {
            concert = await concertCatalogService.GetConcert(basketLine.ConcertId);
            concertsCache.Add(basketLine.ConcertId, concert);
        }

        return basket.Add(basketLine, concert);
    }

    public async Task<Basket> GetBasket(Guid basketId)
    {
        baskets.TryGetValue(basketId, out var basket);
        return new Basket()
        {
            BasketId = basketId,
            NumberOfItems = basket?.Lines?.Count ?? 0,
            UserId = basket?.UserId ?? Guid.Empty
        };

    }

    public async Task<IEnumerable<BasketLine>> GetLinesForBasket(Guid basketId)
    {
        if (!baskets.TryGetValue(basketId, out var basket))
        {
            return new BasketLine[0];
        }
        return basket.Lines;
    }

    public async Task UpdateLine(Guid basketId, BasketLineForUpdate basketLineForUpdate)
    {
        if (baskets.TryGetValue(basketId, out var basket))
        {
            basket.Update(basketLineForUpdate);
        }
    }

    public async Task RemoveLine(Guid basketId, Guid lineId)
    {
        if (baskets.TryGetValue(basketId, out var basket))
        {
            basket.Remove(lineId);
        }
    }

    public Task ClearBasket(Guid basketId)
    {
        if (baskets.TryGetValue(basketId, out var basket))
        {
            basket.Clear();
        }
        return Task.CompletedTask;
    }
}
