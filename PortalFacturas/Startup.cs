using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortalFacturas.Interfaces;
using Refit;

namespace PortalFacturas;

public class Startup
{
    private readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
        _config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();

        // PDF convert
        services
            .AddRefitClient<IPdfApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(_config["ApiPdfConvert:BaseUrl"]));

        // Cookies
        services
            .AddAuthentication("appcookie")
            .AddCookie(
                "appcookie",
                options =>
                {
                    options.LoginPath = "/Index";
                }
            );
        services.AddSession();
        services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
            // app.UseExceptionHandler("/Index");
        }
        app.Use(
            async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    ctx.Request.Path = "/Error";
                    await next();
                }
                else if (ctx.Response.StatusCode == 500)
                {
                    ctx.Request.Path = "/Index";
                    await next();
                }
                else if (ctx.Response.StatusCode == 503)
                {
                    ctx.Request.Path = "/Error";
                    await next();
                }
                else if (ctx.Response.StatusCode == 302)
                {
                    ctx.Request.Path = "/Index";
                    await next();
                }
            }
        );

        app.UseSession();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapRazorPages();
            }
        );
    }
}
