var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddMediatR(c =>
{
    c.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

var app = builder.Build();
app.MapCarter();

app.Run();
