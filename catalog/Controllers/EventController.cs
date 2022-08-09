using Microsoft.AspNetCore.Mvc;
using GloboTicket.Catalog.Repositories;

namespace GloboTicket.Catalog.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventRepository eventRepository;
    private readonly ILogger<EventController> logger;

    public EventController(IEventRepository repository, ILogger<EventController> logger)
    {
        eventRepository = repository;
        this.logger = logger;
    }

    [HttpGet(Name = "GetEvents")]
    public  IEnumerable<Event> GetAll()
    {
      return eventRepository.GetEvents();
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<Event> GetById(Guid id)
    {
        return await eventRepository.GetEventById(id);
    }
}
