using FacebookAuth.Data;
using FacebookAuth.Models;
using FacebookAuth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace FacebookAuth
{
    public class Startup
    {
        public static class FacebookSettings
        {
            public static string AppId { get; set; }
            public static string AppSecret { get; set; }
            public static string Scope { get; set; }
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                FacebookSettings.AppId = facebookOptions.AppId;
                FacebookSettings.AppSecret = facebookOptions.AppSecret;
                FacebookSettings.Scope = Configuration["Facebook:Scope"];

                //facebookOptions.CallbackPath = new PathString("/SignInFacebook");
                facebookOptions.SaveTokens = true;
                //facebookOptions.TokenEndpoint= new PathString("/SignInFacebook");

                facebookOptions.Events.OnCreatingTicket = ctx =>
                {
                    if (ctx.Properties.GetTokens() is List<AuthenticationToken> tokens)
                    {
                        tokens.Add(new AuthenticationToken()
                        {
                            Name = "TicketCreated",
                            Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                        });
                        ctx.Properties.StoreTokens(tokens);
                    }
                    return Task.CompletedTask;
                };
            });

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
