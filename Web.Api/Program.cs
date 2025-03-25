using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Api.Data;
using Web.Api.Extensions;
using Web.Api.Interfaces;
using Web.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddDbContext<IApiDataContext, ApiDataContext>();
builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();