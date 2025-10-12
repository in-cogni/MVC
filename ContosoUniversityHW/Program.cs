using ContosoUniversityHW.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<ContosoUniversiltyContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ContosoUniversiltyContext") ?? throw new InvalidOperationException("Connection string 'ContosoUniversiltyContext' not found.")));

//builder.Services.AddDbContext<UniversityContext>
//    (
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"))
//    );

builder.Services.AddDbContext<UniversityContext>
	(
	options => options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection1"))
	);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

/*using (var scope1 = app.Services.CreateScope())
{
	var services1 = scope1.ServiceProvider;
	try
	{
		var context1 = services1.GetRequiredService<UniversityContext>();
		context1.Database.Migrate();
	}
	catch (Exception ex)
	{
		var logger = services1.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while migrating the database.");
	}
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
///////////////////////////////////////////////////

IServiceScope scope = app.Services.CreateScope();
IServiceProvider services = scope.ServiceProvider;

UniversityContext context = services.GetRequiredService<UniversityContext>();
DbInitializer.Initialize(context);

///////////////////////////////////////////////////

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
