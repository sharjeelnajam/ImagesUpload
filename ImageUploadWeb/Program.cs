using ImageUploadWeb.Data;
using ImageUploadWeb.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ImageUploadWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();

            // Add HTTP client for API communication
            builder.Services.AddHttpClient("ImageUploadAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7153/"); // Update this to match your API URL
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Register services
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
