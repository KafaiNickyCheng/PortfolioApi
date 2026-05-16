using portfolio_api.Interfaces.Service;
using portfolio_api.Interfaces.Services;
using portfolio_api.Middlewares;
using portfolio_api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("Resend");
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PortfolioPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://kafai-portfolio.up.railway.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("PortfolioPolicy");
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();