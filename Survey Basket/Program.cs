
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependancies(builder.Configuration);

//builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//            .AddEntityFrameworkStores<EntityContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
//app.MapIdentityApi<ApplicationUser>();
app.UseCors("FirstPolicy");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

app.Run();
