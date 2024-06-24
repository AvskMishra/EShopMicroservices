#region CREATE BUILDER

var builder = WebApplication.CreateBuilder(args);

#endregion

var assembly = typeof(Program).Assembly;

#region DI (Add Services)

builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssemblies(assembly);
    c.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMarten(opt =>
    {
        opt.Connection(builder.Configuration.GetConnectionString("Database")!);
    }).UseLightweightSessions();

builder.Services.AddCarter();

#endregion

#region APPLICATION BUILD

var app = builder.Build();

#endregion

#region CONFIGURE PIPELINE (app.use METHODS)

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception == null) return;
        var problemDetails = new ProblemDetails
        {
            Title = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.StackTrace
        };
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, exception.Message);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.MapCarter();

#endregion

#region APPLICATION RUN

app.Run();

#endregion
