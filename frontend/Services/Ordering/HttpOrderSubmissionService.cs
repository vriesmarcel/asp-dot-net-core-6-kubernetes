using GloboTicket.Frontend.Models.Api;
using GloboTicket.Frontend.Models.View;
using GloboTicket.Frontend.Services.ShoppingBasket;
using Prometheus;

namespace GloboTicket.Frontend.Services.Ordering;

public class HttpOrderSubmissionService : IOrderSubmissionService
{
    private readonly IShoppingBasketService shoppingBasketService;
    private readonly HttpClient orderingClient;
    private static ICounter ticketsSold = null;
 
    public HttpOrderSubmissionService(IShoppingBasketService shoppingBasketService, HttpClient orderingClient)
    {
        this.shoppingBasketService = shoppingBasketService;
        this.orderingClient = orderingClient;
    }
    public async Task<Guid> SubmitOrder(CheckoutViewModel checkoutViewModel)
    {

        var lines = await shoppingBasketService.GetLinesForBasket(checkoutViewModel.BasketId);
        var order = new OrderForCreation();
        order.Date = DateTimeOffset.Now;
        order.OrderId = Guid.NewGuid();
        order.Lines = lines.Select(line => new OrderLine() { ConcertId = line.ConcertId, Price = line.Price, TicketCount = line.TicketAmount }).ToList();
        order.CustomerDetails = new CustomerDetails()
        {
            Address = checkoutViewModel.Address,
            CreditCardNumber = checkoutViewModel.CreditCard,
            Email = checkoutViewModel.Email,
            Name = checkoutViewModel.Name,
            PostalCode = checkoutViewModel.PostalCode,
            Town = checkoutViewModel.Town,
            CreditCardExpiryDate = checkoutViewModel.CreditCardDate
        };
        SendTelemetryOrderPlaced(lines.Sum(item => item.TicketAmount));
        // make a synchronous call to the ordering microservice
        var response = await orderingClient.PostAsJsonAsync("order", order);
        // can be a validation error - haven't implemented validation yet
        var s = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        return order.OrderId;
    }

    private void SendTelemetryOrderPlaced(int numtickets)
    {
        if (ticketsSold == null)
        {
            ticketsSold =
                Metrics.CreateCounter("globoticket_tickets_sold", "Number of tickets in shopping basket on checkout");
        }

        ticketsSold.Inc(Convert.ToDouble(numtickets));
    }
}
