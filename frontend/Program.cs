using GloboTicket.Frontend.Services;
using GloboTicket.Frontend.Models;
using GloboTicket.Frontend.Services.Ordering;
using GloboTicket.Frontend.Services.ShoppingBasket;
using GloboTicket.Frontend.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Prometheus;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IShoppingBasketService, InMemoryShoppingBasketService>();
builder.Services.AddHttpClient<IConcertCatalogService, ConcertCatalogService>(
    (provider, client) =>{
        client.BaseAddress = new Uri(provider.GetService<IConfiguration>()?["ApiConfigs:ConcertCatalog:Uri"] ?? throw new InvalidOperationException("Missing config"));
    });

builder.Services.AddHttpClient<IOrderSubmissionService, HttpOrderSubmissionService>(
    (provider, client) => {
        client.BaseAddress = new Uri(provider.GetService<IConfiguration>()?["ApiConfigs:Ordering:Uri"] ?? throw new InvalidOperationException("Missing config"));
    });

builder.Services.AddHealthChecks()
   .AddCheck<SlowDependencyHealthCheck>("SlowDependencyDemo", tags: new string[] { "ready" })
   .AddProcessAllocatedMemoryHealthCheck(maximumMegabytesAllocated: 500);

builder.Services.AddHttpClient(Options.DefaultName)
    .UseHttpClientMetrics();

builder.Services.AddSingleton<Settings>();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Turning this off to simplify the running in Kubernetes demo
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ConcertCatalog}/{action=Index}/{id?}");

//map the livelyness and readyness probes
app.MapHealthChecks("/health/ready",
new HealthCheckOptions()
{
    Predicate = reg => reg.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/lively",
new HealthCheckOptions()
{
    Predicate = reg => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpMetrics();
app.UseMetricServer();

app.UseEndpoints(endpoints =>
endpoints.MapMetrics());

app.Run();
