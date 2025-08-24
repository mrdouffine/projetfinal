using Blazored.LocalStorage;
using DinkToPdf;
using DinkToPdf.Contracts;
using GestionConge.Client.Services;
using GestionConge.Components;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Repositories.RepositoriesImpl;
using GestionConge.Components.Services.IServices;
using GestionConge.Components.Services.ServicesImpl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using Npgsql;
using System.Data;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Pour injecter IDbConnection (PostgreSQL)
//builder.Services.AddScoped<IDbConnection>(sp =>
//    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var conn = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    conn.Open(); // On ouvre directement
    return conn;
});

//// Register the DapperContext for database access
//builder.Services.AddSingleton<DapperContext>();

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddControllers(); // <- important
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
// Register the TestRepository
builder.Services.AddScoped<TestRepository>();
// Register the DemandeCongeRepository
builder.Services.AddScoped<IDemandeCongeRepository, DemandeCongeRepository>();
// Register the DemandeCongeService
builder.Services.AddScoped<IDemandeCongeService, GestionConge.Components.Services.ServicesImpl.DemandeCongeServices>();
// Register the UtilisateurRepository and UtilisateurService
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurServices>();
// Register the ValidationRepository and ValidationService
builder.Services.AddScoped<IValidationRepository, ValidationRepository>();
builder.Services.AddScoped<IValidationService, ValidationServices>();
// Register the PlanningCongeRepository and PlanningCongeService
builder.Services.AddScoped<IPlanningCongeRepository, PlanningCongeRepository>();
builder.Services.AddScoped<IPlanningCongeService, PlanningCongeServices>();
// Register the RappelRepository and RappelService
builder.Services.AddScoped<IRappelRepository, RappelRepository>();
builder.Services.AddScoped<IRappelService, RappelServices>();
// Register the AuthService
builder.Services.AddScoped<IAuthService, AuthServices>();
// Register the EmailService
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailService, MailServices>();
// Register the EmailSenderService
builder.Services.AddHostedService<ReminderEmailServices>();
// Register the PDF export service
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IPdfExportService, PdfExportService>();
//Register AuthService from GestionConge.Client
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UtilisateurService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<DemandeCongeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlanningCongeService>();
builder.Services.AddScoped<RappelService>();
builder.Services.AddScoped<ValidationService>();
// Notification service
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHttpClient<UtilisateurService>();


var jwt = builder.Configuration.GetSection("JwtSettings");
var secret = jwt["SecretKey"]!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();


builder.Services.AddCors(opt =>
{
    opt.AddPolicy("client",
        p => p.WithOrigins("https://localhost:7064", "http://localhost:7064", "https://localhost:5001", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});


// 
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true; // Seulement en développement
    options.DisconnectedCircuitMaxRetained = 100;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    options.MaxBufferedUnacknowledgedRenderBatches = 10;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    

}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapControllers(); // <- important pour activer les routes API
app.UseAntiforgery();
app.UseCors("client");
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GestionConge.Client._Imports).Assembly);


app.Run();
