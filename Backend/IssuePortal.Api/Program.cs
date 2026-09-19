using IssuePortal.Api.Data;
using Microsoft.EntityFrameworkCore;
using IssuePortal.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IssuePortalDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IssueService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CommentService>();

builder.Services.AddControllers();



var app = builder.Build();

//// tester i fal det funker med databsen 
/// 
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IssuePortalDbContext>();

    if (dbContext.Database.CanConnect())
    {
        Console.WriteLine("✅ Connected to PostgreSQL!");
    }
    else
    {
        Console.WriteLine("❌ Could not connect to PostgreSQL.");
    }
}


// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();