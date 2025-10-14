using ContosoUniversityHW.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<ContosoUniversiltyContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ContosoUniversiltyContext") ?? throw new InvalidOperationException("Connection string 'ContosoUniversiltyContext' not found.")));

//builder.Services.AddDbContext<UniversityContext>
//    (
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"))
//    );

builder.Services.AddDbContext<UniversityContext>
	(
	options => options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection1")).ConfigureWarnings(warnings =>
			   warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
	);

// Add services to the container.
builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
	IServiceProvider services = scope.ServiceProvider;
	try
	{
		UniversityContext context = services.GetRequiredService<UniversityContext>();
		context.Database.EnsureCreated();

		DbInitializer.Initialize(context);
	}
	catch (Exception ex)
	{
		ILogger logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "DB INIT ERROR: {Message}\n{StackTrace}", ex.Message, ex.StackTrace);
		throw; 
	}
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
///////////////////////////////////////////////////

//IServiceScope scope = app.Services.CreateScope();
//IServiceProvider services = scope.ServiceProvider;

//UniversityContext context = services.GetRequiredService<UniversityContext>();
//DbInitializer.Initialize(context);

///////////////////////////////////////////////////

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
