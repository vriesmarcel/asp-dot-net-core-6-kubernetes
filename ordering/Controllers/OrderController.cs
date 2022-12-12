using GloboTicket.Ordering.Model;
using GloboTicket.Ordering.Services;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace GloboTicket.Ordering.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> logger;
    private readonly EmailSender emailSender;
    private static ICounter ordersCompleted = null;
    public OrderController(ILogger<OrderController> logger, EmailSender emailSender)
    {
        this.logger = logger;
        this.emailSender = emailSender;
    }

    [HttpPost("", Name = "SubmitOrder")]
    public IActionResult Submit(OrderForCreation order)
    {
        logger.LogInformation($"Received a new order from {order.CustomerDetails.Name}");
        RegisterMetricsOrderProcessed();
        emailSender.SendEmailForOrder(order);
        return Ok();
    }

    private void RegisterMetricsOrderProcessed()
    {
        if (ordersCompleted == null)
        { 
            ordersCompleted = 
                Metrics.CreateCounter("globoticket_orders_processed", "Number of orders completed"); 
        }

        ordersCompleted.Inc();
    }
}
