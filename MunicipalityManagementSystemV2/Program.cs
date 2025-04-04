using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MunicipalityManagementSystemV2.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MunicipalityManagementSystemV2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MunicipalityManagementSystemV2Context") ?? throw new InvalidOperationException("Connection string 'MunicipalityManagementSystemV2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
