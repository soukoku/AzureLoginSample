using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace AzureLoginSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(op =>
            {
                op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = "Azure";
            })
                .AddCookie()
                .AddOpenIdConnect("Azure", options =>
                {
                    options.Authority = $"{Configuration["AzureAd:Instance"]}{Configuration["AzureAd:TenantId"]}/v2.0/";

                    options.ClientId = Configuration["AzureAd:ClientId"];
                    //options.ClientSecret = Configuration["AzureAd:ClientSecret"]; // use if using code response type

                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.GetClaimsFromUserInfoEndpoint = false;

                    options.Scope.Add("openid");
                    //options.Scope.Add("profile");
                    options.Scope.Add("email");

                    // reduce saved cookie size by removing claims
                    options.ClaimActions.Add(new DeleteClaimAction("preferred_username"));
                    // may be necessary, to test
                    options.ClaimActions.Add(new DeleteClaimAction("aio"));
                    options.ClaimActions.Add(new DeleteClaimAction("oid"));
                    options.ClaimActions.Add(new DeleteClaimAction("tid"));
                    options.ClaimActions.Add(new DeleteClaimAction("uti"));

                    options.CallbackPath = Configuration["AzureAd:CallbackPath"];
                    options.SignedOutCallbackPath = Configuration["AzureAd:SignedOutCallbackPath"];

                    options.ClaimsIssuer = "AzureAd";

                    //options.Events.OnUserInformationReceived = ctx =>
                    //{
                    //    Debugger.Break();
                    //    return Task.CompletedTask;
                    //};

                    //options.Events.OnTokenResponseReceived = ctx =>
                    //{
                    //    Debugger.Break();
                    //    return Task.CompletedTask;
                    //};

                });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
