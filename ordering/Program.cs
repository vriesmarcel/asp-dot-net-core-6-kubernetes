using GloboTicket.Ordering.Services;
using Microsoft.Extensions.Options;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<EmailSender>();

builder.Services.AddHealthChecks()
                        .ForwardToPrometheus();
builder.Services.AddHttpClient(Options.DefaultName)
    .UseHttpClientMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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
