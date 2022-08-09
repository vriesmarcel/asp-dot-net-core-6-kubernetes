using GloboTicket.Frontend.Extensions;
using GloboTicket.Frontend.Models.Api;

namespace GloboTicket.Frontend.Services;

public class EventCatalogService : IEventCatalogService
{
    private readonly HttpClient client;

    public EventCatalogService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<IEnumerable<Event>> GetAll()
    {
        HttpResponseMessage response = await client.GetAsync("event");
        return await response.ReadContentAs<List<Event>>();
    }

    public async Task<Event> GetEvent(Guid id)
    {
        HttpResponseMessage response = await client.GetAsync($"event/{id}");
        return await response.ReadContentAs<Event>();
    }
}
