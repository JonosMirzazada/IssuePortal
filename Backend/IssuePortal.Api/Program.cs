var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/issues", () =>
{
    return new[]
    {
        new
        {
            Id = 1,
            Title = "Login fungerar inte",
            Status = "Open",
            Priority = "High"
        },
        new
        {
            Id = 2,
            Title = "Dashboard behöver fixas",
            Status = "In Progress",
            Priority = "Medium"
        }
    };
});

app.Run();