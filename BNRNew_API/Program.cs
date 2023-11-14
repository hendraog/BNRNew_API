using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BNRNew_API.Controllers;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.dto;

var builder = WebApplication.CreateBuilder(args);

var config = ConfigHelper.loadConfig<AppConfig>(new ConfigurationBuilder(),"dev");

builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<AppConfig>(config);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<MyDBContext>(options =>
{
    options.UseSqlite(@"Data Source=D:\test.db").EnableSensitiveDataLogging();
});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                      }) ;
});

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var message = string.Join(" | ", context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

        return new BadRequestObjectResult(new BaseDtoResponse
        { 
            message = message,
            error_field = context.ModelState.Keys.ToList()
        });
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IgnoreNullValues = true;
});


var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


/*
using (ApplicationContext db = new ApplicationContext())
{

    var users = db.Users.ToList();
    Console.WriteLine("Users list:");
    foreach (User u in users)
    {
        Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
    }

}*/

app.UseMiddleware<RequestResponseLoggingMiddleware<JWTModel>>(config.jwtSecret);


app.UseDefaultFiles(); // Enables default file mapping on the web root.
app.UseStaticFiles(); // Marks files on the web root as servable.


app.UseAuthorization();

app.MapControllers();

app.Run();

