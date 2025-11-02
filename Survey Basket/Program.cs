using Hangfire.Dashboard;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependancies(builder.Configuration);
var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard("/jobs",new DashboardOptions
{
    Authorization = 
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User=app.Configuration.GetValue<string>("HangfireSettings:User"),
            Pass=app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
     ],
    DashboardTitle = "Survey Basket Jobs",
    //IsReadOnlyFunc=(DashboardContext context)=>true
});
var scopeFactory=app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var PollNotifiaction=scope.ServiceProvider.GetRequiredService<INotificationService>();
RecurringJob.AddOrUpdate("SendNewPollNotifiaction", () => PollNotifiaction.SendNewPollNotifiaction(null), Cron.Daily);


app.MapHealthChecks("Health",new HealthCheckOptions
{
    ResponseWriter=UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("Health-database",new HealthCheckOptions
{
    ResponseWriter=UIResponseWriter.WriteHealthCheckUIResponse,
   Predicate=x=> x.Tags.Contains("database")
});
app.UseRateLimiter();

//app.MapIdentityApi<ApplicationUser>();
app.UseCors("FirstPolicy");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();
app.Run();
