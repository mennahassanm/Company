using System.Configuration;
using Company.MVC.BLL;
using Company.MVC.BLL.Interfaces;
using Company.MVC.BLL.Repositories;
using Company.MVC.DAL.Data.Contexts;
using Company.MVC.DAL.Models;
using Company.MVC.PL.Helpers;
using Company.MVC.PL.Mapping;
using Company.MVC.PL.Services;
using Company.MVC.PL.Settings;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Company.MVC03.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // Allow DI For DepartmentRepositories
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Allow DI For DepartmentRepositories

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            
            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }); // Allow DI For CompanyDbContext


            builder.Services.AddAutoMapper(M => M.AddProfile(new EmployeeProfile()));
            builder.Services.AddAutoMapper(M => M.AddProfile(new DepartmentProfile()));

            builder.Services.Configure<Mailsettings>(builder.Configuration.GetSection("Mailsettings"));


            builder.Services.AddScoped<IMailService, MailService>();

            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection(nameof(TwilioSettings)));

            builder.Services.AddScoped<ITwilioService, TwilioService>();

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                              .AddEntityFrameworkStores<CompanyDbContext>()
                              .AddDefaultTokenProviders();


            // Life Time
            //builder.Services.AddScoped();    // Create Object Life Per Request - UnReachable Object 
            //builder.Services.AddTransient(); // Create Object Life Per Operation 
            //builder.Services.AddSingleton(); // Create Object Life Per Application

            builder.Services.AddScoped<IScopedService, ScopedService>(); // Per Request
            builder.Services.AddTransient<ITransentService, TransentService>(); // Per Operation
            builder.Services.AddSingleton<ISingletonService, SingletonService>(); // Per Application

            

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/SignIn";
                config.AccessDeniedPath = "/Account/AccessDenied"; //<=

            });

            builder.Services.AddAuthentication(O =>
            {
                O.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                O.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddGoogle(O =>
            {
                O.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                O.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

            });

            builder.Services.AddAuthentication(O =>
            {
                O.DefaultAuthenticateScheme = FacebookDefaults.AuthenticationScheme;
                O.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            }).AddFacebook(O =>
            {
                O.ClientId = builder.Configuration["Authentication:facebook:ClientId"];
                O.ClientSecret = builder.Configuration["Authentication:facebook:ClientSecret"];

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}