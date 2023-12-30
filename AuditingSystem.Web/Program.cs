using AuditingSystem.Database;
using AuditingSystem.Entities.AuditProcess;
using AuditingSystem.Entities.Lockups;
using AuditingSystem.Entities.RiskAssessments;
using AuditingSystem.Entities.Setup;
using AuditingSystem.Services.Implements;
using AuditingSystem.Services.Interfaces;
using AuditingSystem.Web.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuditingSystemDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditingSystemConnection"));
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Services.AddScoped<IBaseRepository<User, int>, BaseRepository<User, int>>();
builder.Services.AddScoped<IBaseRepository<Year, int>, BaseRepository<Year, int>>();
builder.Services.AddScoped<IBaseRepository<Role, int>, BaseRepository<Role, int>>();

builder.Services.AddScoped<IBaseRepository<Industry, int>, BaseRepository<Industry, int>>();
builder.Services.AddScoped<IBaseRepository<Company, int>, BaseRepository<Company, int>>();
builder.Services.AddScoped<IBaseRepository<Department, int>, BaseRepository<Department, int>>();
builder.Services.AddScoped<IBaseRepository<Function, int>, BaseRepository<Function, int>>();
builder.Services.AddScoped<IBaseRepository<Activity, int>, BaseRepository<Activity, int>>();
builder.Services.AddScoped<IBaseRepository<Objective, int>, BaseRepository<Objective, int>>();
builder.Services.AddScoped<IBaseRepository<Tasks, int>, BaseRepository<Tasks, int>>();
builder.Services.AddScoped<IBaseRepository<Practice, int>, BaseRepository<Practice, int>>();

builder.Services.AddScoped<IBaseRepository<RiskIdentification, int>, BaseRepository<RiskIdentification, int>>();
builder.Services.AddScoped<IBaseRepository<Control, int>, BaseRepository<Control, int>>();
builder.Services.AddScoped<IBaseRepository<RiskAssessmentList, int>, BaseRepository<RiskAssessmentList, int>>();

builder.Services.AddScoped<IBaseRepository<RiskCategory, int>, BaseRepository<RiskCategory, int>>();
builder.Services.AddScoped<IBaseRepository<RiskImpact, int>, BaseRepository<RiskImpact, int>>();
builder.Services.AddScoped<IBaseRepository<RiskLikehood, int>, BaseRepository<RiskLikehood, int>>();
builder.Services.AddScoped<IBaseRepository<ControlType, int>, BaseRepository<ControlType, int>>();
builder.Services.AddScoped<IBaseRepository<ControlProcess, int>, BaseRepository<ControlProcess, int>>();
builder.Services.AddScoped<IBaseRepository<ControlFrequency, int>, BaseRepository<ControlFrequency, int>>();
builder.Services.AddScoped<IBaseRepository<ControlEffectiveness, int>, BaseRepository<ControlEffectiveness, int>>();


builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<HttpClient>();


builder.Services.AddScoped<AppSession>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
