using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Serilog;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Functions;
using TimeTrackerRepo.Models;
using TimeTrackerRepo.Services;
using TimeTrackerRepo.Services.Reports;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

Log.Logger = new Serilog.LoggerConfiguration().MinimumLevel.Information().Enrich.FromLogContext().WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Month).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<TimeTrackerContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("TimeTrackerContext"))
);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddRazorPages()
    .AddMvcOptions(options => { })
    .AddMicrosoftIdentityUI();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddLogging();

builder.Services.AddScoped<IDataFunctions, DataFunctions>();
builder.Services.AddHostedService<FrozenDateWatcher>();
builder.Services.AddScoped<IUserFunctions, UserFunctions>();

builder.Services.AddKendo();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromHours(8));
builder.Services.AddScoped<TimeTrackerRepo.Services.Reports.Legacy.SQLFunctions>();
builder.Services.AddScoped<TimeTrackerRepo.Services.Reports.MissingHoursReportService>();
builder.Services.AddScoped<TimeTrackerRepo.Services.Reports.MissingHoursExcelExporter>();
builder.Services.AddScoped<TimeTrackerRepo.Services.Reports.ExtractReportService>();
builder.Services.AddScoped<TimeTrackerRepo.Services.Reports.ExtractExcelExporter>();
builder.Services.AddScoped<WipDetailReportService>();
builder.Services.AddScoped<WipDetailExcelExporter>();
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.UseSession();

app.MapRazorPages();
app.MapControllers();
app.MapHub<FrozenDateHub>("/hubs/frozenDate");
app.Run();
