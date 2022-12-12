using GloboTicket.Catalog;
using GloboTicket.Catalog.Repositories;
using GloboTicket.Services.EventCatalog.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;
// trigger build
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
// add DB context here
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets("3285d194-4d94-4fda-8e4f-5a6971d28b64");

builder.Services.AddDbContext<EventCatalogDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IConcertRepository, ConcertRepository>();

builder.Services.AddHttpClient(Options.DefaultName)
    .UseHttpClientMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<EventCatalogDbContext>();
        context.Database.EnsureCreated();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.UseHttpMetrics();
app.UseMetricServer();

app.UseEndpoints(endpoints =>
endpoints.MapMetrics());

app.Run();
