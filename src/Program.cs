global using Api.Services.UserService;
global using Api.Data;
global using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

{
    string AllowedSpecificOrigins = "allowedSpecificOrigins";
    var services = builder.Services;
    
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddScoped<IUserService, UserService>();
    services.AddHttpContextAccessor();
    
    services.AddDbContext<TodoContext>(opt =>
        opt.UseInMemoryDatabase("TodoList"));

    services.AddSwaggerGen(options => 
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

    services.AddCors(options =>
    {
        options.AddPolicy(
            name: AllowedSpecificOrigins,
            builder =>
            {
                builder.AllowAnyOrigin();
            }
        );
    });
}

var app = builder.Build();

{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

app.Run();
