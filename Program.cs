using OptimaJet.Workflow.Core.Runtime;
using WorkflowEngineMVC.Data;
using WorkflowEngineMVC.Models;
using WorkflowLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IWorkflowActionProvider, WorkflowActionProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

WorkflowInit.ConnectionString = app.Configuration.GetConnectionString("DefaultConnection");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Designer}/{action=Index}/{id?}");

app.Run();
