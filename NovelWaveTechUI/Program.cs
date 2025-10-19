using Application.ApiHttpClient;
using NovelWaveTechUI.BaseURL;
using NovelWaveTechUI.Chat;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<HttpClient, HttpClient>();
//builder.Services.AddSingleton<IHttpContextAccessor, IHttpContextAccessor>();
builder.Services.AddSingleton<IHttpClients, HttpClients>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Admin}/{action=Index2}/{id?}");

    // Map SignalR hub
    endpoints.MapHub<ChatHub>("/chathub");
});


app.Run();
