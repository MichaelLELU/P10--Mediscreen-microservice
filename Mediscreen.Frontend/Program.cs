using Mediscreen.Frontend.Services;
using Mediscreen.Frontend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string gatewayBaseUrl =
    builder.Configuration["Gateway:BaseUrl"]
    ?? throw new InvalidOperationException(
        "L'adresse de la Gateway est manquante.");

builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
});

builder.Services.AddScoped<IPatientService, PatientService>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patients}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
