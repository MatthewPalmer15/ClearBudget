using ClearBudget.Application;
using ClearBudget.Database;
using ClearBudget.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
#if DEBUG
builder.Configuration.AddJsonFile("appsettings.Debug.json", optional: true);
#else
builder.Configuration.AddJsonFile("appsettings.Release.json", optional: true);
#endif

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetService<Microsoft.AspNetCore.Components.NavigationManager>();
    if (nav is not null)
    {
        try
        {
            var baseUri = nav.BaseUri;          // will throw if not initialized
            return new HttpClient { BaseAddress = new Uri(baseUri) };
        }
        catch (InvalidOperationException)
        {
            // RemoteNavigationManager not initialized yet; fall through
        }
    }

    var http = sp.GetService<IHttpContextAccessor>()?.HttpContext;
    if (http is not null && http.Request.Host.HasValue)
    {
        var origin = $"{http.Request.Scheme}://{http.Request.Host}/";
        return new HttpClient { BaseAddress = new Uri(origin) };
    }

    return sp.GetRequiredService<IHttpClientFactory>().CreateClient();
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = ".ClearBudget.Auth";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/forbidden";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddMudServices();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "dpkeys")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
    }
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClearBudget.Web.Client._Imports).Assembly);

app.Run();
