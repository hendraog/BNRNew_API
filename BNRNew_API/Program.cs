using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BNRNew_API.Controllers;
using BNRNew_API.Controllers.user.dto;
using BNRNew_API.Entities;
using BNRNew_API.config;
using BNRNew_API.utils;
using BNRNew_API.Controllers.dto;
using BNRNew_API.Controllers.golongan;
using BNRNew_API.Controllers.golonganplat;
using BNRNew_API.Controllers.ticket;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;
using NPOI.OpenXml4Net.OPC;
using Microsoft.Extensions.Configuration;
using NPOI.XWPF.UserModel;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();



var config = ConfigHelper.loadConfig<AppConfig>(new ConfigurationBuilder(),"dev");

var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();

Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(configuration)
          .CreateLogger();

Log.Information("Starting up");


builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGolonganService, GolonganService>();
builder.Services.AddScoped<IGolonganPlatService, GolonganPlatService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ISequenceService, SequenceService>();
builder.Services.AddScoped<ICargoManifestService, CargoManifestService>();
builder.Services.AddSingleton<AppConfig>(config);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new ()
        {
            Title = "BNRAPI - V1",
            Version = "v1"
        }
    );
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "BNRApi.xml");
    c.IncludeXmlComments(filePath);
});

builder.Services.AddDbContext<MyDBContext>(options =>
{
    options.UseSqlite(@"Data Source=data.db").EnableSensitiveDataLogging();
});
builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                       
}) ;
});
//                            .SetIsOriginAllowedToAllowWildcardSubdomains()

// .AllowCredentials();

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
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});




var app = builder.Build();


app.UseCors(MyAllowSpecificOrigins);

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (ctx, elapsed, ex) =>
    {
        if (ex != null || ctx.Response.StatusCode > 499)
            return LogEventLevel.Error;
        
        if (elapsed > TimeSpan.FromSeconds(3).TotalMilliseconds)
            return LogEventLevel.Warning;

        return LogEventLevel.Information;
    };

});




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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/web"
});

app.UseRouting(); 

app.UseEndpoints(endpoints =>
{
    endpoints.MapFallbackToFile("/index.html");
});

app.UseAuthorization();

app.MapControllers();

app.Run();

