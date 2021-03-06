using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SalesWebMvc.Services;


var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("SalesWebMvcContext");

builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder => builder.MigrationsAssembly("SalesWebMvc")));

builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<SalesRecordService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddTransient<SeedingService>();




// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


SeedData(app);
//Seed Data
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedingService>();
        service.Seed();
    }
}

// Set location

var enUS = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS }
};

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    using (var scope = app.Services.CreateScope())
    {
        /*var dbContext = scope.ServiceProvider.GetRequiredService<SalesWebMvcContext>();
        // use context
        SeedingService ss = new SeedingService(dbContext);
        ss.Seed();*/
    }

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


