
#region CREATE BUILDER
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

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

builder.Services.AddCarter();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

#endregion

#region APPLICATION BUILD

var app = builder.Build();

#endregion

#region CONFIGURE PIPELINE (app.use METHODS)

app.UseExceptionHandler(options => { });

app.MapCarter();

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    
#endregion

#region APPLICATION RUN

app.Run();

#endregion
