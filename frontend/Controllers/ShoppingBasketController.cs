using GloboTicket.Frontend.Extensions;
using GloboTicket.Frontend.Models;
using GloboTicket.Frontend.Models.Api;
using GloboTicket.Frontend.Models.View;
using GloboTicket.Frontend.Services.ShoppingBasket;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace GloboTicket.Frontend.Controllers;

public class ShoppingBasketController : Controller
{
    private readonly IShoppingBasketService basketService;
    private readonly Settings settings;
    private readonly ILogger<ShoppingBasketController> logger;
    private readonly TelemetryClient telemetryClient;

    public ShoppingBasketController(IShoppingBasketService basketService, TelemetryClient telemetryClient, Settings settings, ILogger<ShoppingBasketController> logger)
    {
        this.basketService = basketService;
        this.settings = settings;
        this.logger = logger;
        this.telemetryClient = telemetryClient;
    }

    public async Task<IActionResult> Index()
    {
        var basketLines = await basketService.GetLinesForBasket(Request.Cookies.GetCurrentBasketId(settings));
        var lineViewModels = basketLines.Select(bl => new BasketLineViewModel
        {
            LineId = bl.BasketLineId,
            EventId = bl.EventId,
            EventName = bl.Event.Name,
            Date = bl.Event.Date,
            Price = bl.Price,
            Quantity = bl.TicketAmount
        }
        );
        return View(lineViewModels);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLine(BasketLineForCreation basketLine)
    {
        SendAppInsightsTelemetryAddLine(basketLine);
        var basketId = Request.Cookies.GetCurrentBasketId(settings);
        var newLine = await basketService.AddToBasket(basketId, basketLine);
        Response.Cookies.Append(settings.BasketIdCookieName, newLine.BasketId.ToString());

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateLine(BasketLineForUpdate basketLineUpdate)
    {
        SendAppInsightsTelemetryUpdateLine(basketLineUpdate);    
        var basketId = Request.Cookies.GetCurrentBasketId(settings);
        await basketService.UpdateLine(basketId, basketLineUpdate);
        return RedirectToAction("Index");
    } 

    public async Task<IActionResult> RemoveLine(Guid lineId)
    {
        var basketId = Request.Cookies.GetCurrentBasketId(settings);
        await basketService.RemoveLine(basketId, lineId);
        return RedirectToAction("Index");
    }

    private void SendAppInsightsTelemetryAddLine(BasketLineForCreation basketLine)
    {
        MetricTelemetry telemetry = new MetricTelemetry();
        telemetry.Name = "Items in basket";
        telemetry.Sum = basketLine.TicketAmount;
        telemetryClient.TrackMetric(telemetry);
    }

    private void SendAppInsightsTelemetryUpdateLine(BasketLineForUpdate basketLine)
    {
        MetricTelemetry telemetry = new MetricTelemetry();
        telemetry.Name = "Items in basket";
        telemetry.Sum = basketLine.TicketAmount;
        telemetryClient.TrackMetric(telemetry);
    }
}
