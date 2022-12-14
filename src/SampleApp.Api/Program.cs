using SampleApp.Api;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEFDbContext(builder.Configuration);
builder.Services.AddServices();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressInferBindingSourcesForParameters = true;
    });

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAppSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();

app.UseCors(x =>
{
    //x.WithOrigins("http://localhost:8080")
    x.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
             //.AllowCredentials();
});

//app.UseCors(x => x.AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.UseAppSwagger();

app.MapAppControllers();

app.Run();
//TODO
// - Implement FE
// - Add support for mysql and postgres
// - add docker - compose to run FE and BE
// - Add EntityDescription to Approval
// - Add Integration test to test approval properly