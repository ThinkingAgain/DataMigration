using Microsoft.EntityFrameworkCore;
using MigDashboard.Data;
using MigDashboard.Hubs;

var builder = WebApplication.CreateBuilder(args);

var mysqlConnectionString = builder.Configuration.GetConnectionString("QueryContext") ?? throw new InvalidOperationException("Connection string 'QueryContext' not found.");
builder.Services.AddDbContext<QueryContext>(options =>
    options.UseMySql(
        mysqlConnectionString,
        ServerVersion.AutoDetect(mysqlConnectionString)
        )
    );

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<TaskHub>("/taskhub");

app.Run();
