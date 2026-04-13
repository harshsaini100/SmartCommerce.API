using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCommerce.API.Data;
using SmartCommerce.API.Repositories.Implementations;
using SmartCommerce.API.Repositories.Interfaces;
using SmartCommerce.API.Services.Implementations;
using SmartCommerce.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.InvalidModelStateResponseFactory = context =>
//    {
//        var errors = context.ModelState
//            .Where(x => x.Value.Errors.Count > 0)
//            .ToDictionary(
//                kvp => kvp.Key,
//                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
//            );

//        return new BadRequestObjectResult(new
//        {
//            success = false,
//            errors
//        });
//    };
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
    

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseAuthorization();


app.MapControllers();

app.Run();
