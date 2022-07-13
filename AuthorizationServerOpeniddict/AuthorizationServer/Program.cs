using AuthorizationServer.Test;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => options.LoginPath = "/account/login");

builder.Services.AddDbContext<DbContext>(options =>
{
    options.UseInMemoryDatabase(nameof(DbContext));
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<DbContext>())
    .AddServer(options =>
    {
        options
            .AllowClientCredentialsFlow()
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange()
            .AllowRefreshTokenFlow()

            .SetTokenEndpointUris("/connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetUserinfoEndpointUris("/connect/userinfo")

            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey()
            .DisableAccessTokenEncryption()
            .RegisterScopes("api")

            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough();
    });

builder.Services.AddHostedService<TestData>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.Run();
