using Microsoft.EntityFrameworkCore;
using my_api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<Idao, Dao>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


using (ApplicationContext db = new ApplicationContext())
{
    User user1 = new User { Name = "Tom", Age = 33 };
    User user2 = new User { Name = "Alice", Age = 26 };

    db.Users.AddRange(user1, user2);
    db.SaveChanges();
}

using (ApplicationContext db = new ApplicationContext())
{

    var users = db.Users.ToList();
    Console.WriteLine("Users list:");
    foreach (User u in users)
    {
        Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
    }

}



app.UseAuthorization();

app.MapControllers();

app.Run();
