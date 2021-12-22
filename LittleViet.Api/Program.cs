using LittleViet.Data.Global;
using LittleViet.Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LittleVietContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("LittleVietContext")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

StartupConfiguration.Configure(builder.Services);

//builder.Services.AddScoped<UnitOfWork>()
//                .AddScoped<IUnitOfWork, UnitOfWork>()
//                .AddScoped<IAccountDomain, AccountDomain>();
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




