#region CREATE BUILDER

using BuildingBlocks.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

#endregion

var assembly = typeof(Program).Assembly;

#region DI (Add Services)

builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssemblies(assembly);
    c.AddOpenBehavior(typeof(ValidationBehavior<,>));
    c.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMarten(opt =>
    { 
        opt.Connection(builder.Configuration.GetConnectionString("Database")!);
    }).UseLightweightSessions();

builder.Services.AddCarter();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

#endregion

#region APPLICATION BUILD

var app = builder.Build();

#endregion

#region CONFIGURE PIPELINE (app.use METHODS)

app.UseExceptionHandler(options => { });

app.MapCarter();

#endregion

#region APPLICATION RUN

app.Run();

#endregion
