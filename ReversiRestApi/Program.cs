using Microsoft.EntityFrameworkCore;
using ReversiRestApi.DAL;
using ReversiRestApi.Models.Database;
using ReversiRestApi.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "Data Source=<SQLSource>; Initial Catalog=ReversiDbRestApi; User ID=SA; Password=<SQLPass>";
connectionString = connectionString.Replace("<SQLSource>", Environment.GetEnvironmentVariable("SQLSource"));
connectionString = connectionString.Replace("<SQLPass>", Environment.GetEnvironmentVariable("SQLPass"));

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

const string MyCors = "_myCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCors, builder =>
    {
        builder.WithOrigins("http://localhost", "http://localhost:5000", "http://localhost:3000", "http://localhost:53337", "http://vincent.hbo-ict.org")
            .WithMethods("GET", "PUT", "POST")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var dbContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
builder.Services.AddSingleton(typeof(ISpelRepository), new SpelAccessLayer(dbContext));
dbContext.Database.Migrate();

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

