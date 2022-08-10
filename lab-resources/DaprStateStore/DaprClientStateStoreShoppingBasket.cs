using Dapr.Client;
using GloboTicket.Frontend.Models;
using GloboTicket.Frontend.Models.Api;
using GloboTicket.Frontend.Services.ShoppingBasket;

namespace GloboTicket.Frontend.Services.ShoppingBasket;

public class DaprClientStateStoreShoppingBasket : IShoppingBasketService
{
    private readonly DaprClient daprClient;
    private readonly IConcerCatalogService concertCatalogService;
    private readonly Settings settings;
    private readonly ILogger<DaprClientStateStoreShoppingBasket> logger;
    private const string stateStoreName = "shopstate";

    public DaprClientStateStoreShoppingBasket(
        DaprClient daprClient,
        IConcertCatalogService concertCatalogService,
        Settings settings,
        ILogger<DaprClientStateStoreShoppingBasket> logger)
    {
        this.daprClient = daprClient;
        this.concertCatalogService = concertCatalogService;
        this.settings = settings;
        this.logger = logger;
    }

    public async Task<BasketLine> AddToBasket(Guid basketId, BasketLineForCreation basketLineForCreation)
    {
        logger.LogInformation($"ADD TO BASKET {basketId}");
        var basket = await GetBasketFromStateStore(basketId);
        var concert = await GetConcertFromStateStore(basketLineForCreation.ConcertId);

        var basketLine = new BasketLine()
        {
            ConcertId = basketLineForCreation.ConcertId,
            TicketAmount = basketLineForCreation.TicketAmount,
            Concert = concert,
            BasketId = basket.BasketId,
            BasketLineId = Guid.NewGuid(),
            Price = basketLineForCreation.Price
        };
        basket.Lines.Add(basketLine);
        logger.LogInformation($"SAVING BASKET {basket.BasketId}");
        await SaveBasketToStateStore(basket);
        return basketLine;
    }

    public async Task<Basket> GetBasket(Guid basketId)
    {
        logger.LogInformation($"GET BASKET {basketId}");
        var basket = await GetBasketFromStateStore(basketId);

        return new Basket() { BasketId = basketId, NumberOfItems = basket.Lines.Count, UserId = basket.UserId };
    }

    public async Task<IEnumerable<BasketLine>> GetLinesForBasket(Guid basketId)
    {
        var basket = await GetBasketFromStateStore(basketId);
        return basket.Lines;
    }

    public async Task RemoveLine(Guid basketId, Guid lineId)
    {
        var basket = await GetBasketFromStateStore(basketId);
        var index = basket.Lines.FindIndex(bl => bl.BasketLineId == lineId);
        if (index >= 0) basket.Lines.RemoveAt(index);
        await SaveBasketToStateStore(basket);
    }
    public async Task UpdateLine(Guid basketId, BasketLineForUpdate basketLineForUpdate)
    {
        var basket = await GetBasketFromStateStore(basketId);
        var index = basket.Lines.FindIndex(bl => bl.BasketLineId == basketLineForUpdate.LineId);
        basket.Lines[index].TicketAmount = basketLineForUpdate.TicketAmount;
        await SaveBasketToStateStore(basket);
    }

    private async Task SaveBasketToStateStore(StateStoreBasket basket)
    {
        var key = $"basket-{basket.BasketId}";
        await daprClient.SaveStateAsync(stateStoreName, key, basket);
        logger.LogInformation($"Created new basket in state store {key}");
    }

    private async Task SaveConcertToStateStore(Concert concert)
    {
        var key = $"concert-{@concert.ConcertId}";
        logger.LogInformation($"Saving concert to state store {key}");
        await daprClient.SaveStateAsync(stateStoreName, key, concert);
    }


    private async Task<StateStoreBasket> GetBasketFromStateStore(Guid basketId)
    {
        var key = $"basket-{basketId}";
        var basket = await daprClient.GetStateAsync<StateStoreBasket>(stateStoreName, key);
        if (basket == null)
        {
            if (basketId == Guid.Empty) basketId = Guid.NewGuid();
            logger.LogInformation($"CREATING NEW BASKET {basketId}");
            basket = new StateStoreBasket();
            basket.BasketId = basketId;
            basket.UserId = settings.UserId;
            basket.Lines = new List<BasketLine>();
            await SaveBasketToStateStore(basket);
        }
        return basket;
    }

    private async Task<Concert> GetConcertFromStateStore(Guid concertId)
    {
        var key = $"concert-{concertId}";
        var concert = await daprClient.GetStateAsync<Concert>(stateStoreName, key);

        if (concert != null)
        {
            logger.LogInformation("Using cached concert");
        }
        else
        {
            concert = await concertCatalogService.GetConcert(concertId);
            await SaveConcertToStateStore(concert);
        }
        return concert;
    }

    public async Task ClearBasket(Guid basketId)
    {
        var basket = await GetBasketFromStateStore(basketId);
        basket.Lines.Clear();
        await SaveBasketToStateStore(basket);
    }
}
