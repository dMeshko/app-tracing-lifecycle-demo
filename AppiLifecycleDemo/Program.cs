using System.Diagnostics;
using System.Reflection;
using AppiLifecycleDemo.Consumers;
using AppiLifecycleDemo.Filters;
using AppiLifecycleDemo.Observers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<SaySomethingConsumer>(x => x.BaseAddress = new Uri("http://localhost:5159/"));

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.SetInMemorySagaRepositoryProvider();

    var entryAssembly = Assembly.GetEntryAssembly();
    x.AddConsumers(entryAssembly);
    x.AddSagaStateMachines(entryAssembly);
    x.AddSagas(entryAssembly);
    x.AddActivities(entryAssembly);
    //x.AddPublishObserver<PublishObserver>();
    //x.AddConsumeObserver<ConsumeObserver>();

    x.UsingInMemory((context, cfg) =>
    {
        cfg.UsePublishFilter(typeof(PublishFilter<>), context);
        cfg.UseConsumeFilter(typeof(ConsumeFilter<>), context);

        cfg.ConfigureEndpoints(context);
    });
});

//Activity.DefaultIdFormat = ActivityIdFormat.W3C;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
