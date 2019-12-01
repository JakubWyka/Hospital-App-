using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DinkToPdf;
using DinkToPdf.Contracts;
using Hospital.Controllers;
using Hospital.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Hospital
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
            services.AddControllersWithViews();
            services.AddMvc();

            services.AddDistributedMemoryCache();

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Hospital;Trusted_Connection=True;";
                options.SchemaName = "dbo";
                options.TableName = "TestCache";
            });

            services.AddDbContext<HostpitalContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 211;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
           // context.LoadUnmanagedLibrary(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Swagger Sample",
                    Version = "v1"
                    
                });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<HospitalContext>();
                context.Database.EnsureCreated();
                
                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS dbo.Appointments");
                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS dbo.Patients");
                context.Database.ExecuteSqlCommand("DROP TABLE IF EXISTS dbo.Doctors");



                

                var databaseCreator = context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                databaseCreator.CreateTables();
                CreateRoles(services).Wait();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Sample");
            });

        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Admin", "Patient", "Doctor" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            AddUser(serviceProvider, "root@root.pl", "root12", "Admin").Wait();
            AddUser(serviceProvider, "a1@a1.pl", "123123", "Patient", "Patient1").Wait();
            AddUser(serviceProvider, "a2@a2.pl", "123123", "Patient", "Patient2").Wait();
            AddUser(serviceProvider, "b1@b1.pl", "123123", "Doctor", "Doctor1").Wait();
            AddUser(serviceProvider, "b2@b2.pl", "123123", "Doctor", "Doctor2").Wait();

        }

        private async Task AddUser(IServiceProvider serviceProvider, string name, string pass, string role, string fullName = "Def")
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser
            {
                UserName = name,
                Email = name,
            };
            string userPWD = pass;

            var _user = await UserManager.FindByNameAsync(user.UserName);
            if (_user != null) await UserManager.DeleteAsync(_user);
            _user = null;

            if (_user == null)
            {
                var createUser = await UserManager.CreateAsync(user, userPWD);
                if (createUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user, role);
                    if (role == "Doctor")
                    {
                        var doctor = new Models.Doctor { name = fullName };
                        doctor.userId = user.Id;
                        var context = serviceProvider.GetRequiredService<HospitalContext>();
                        context.Doctors.Add(doctor);
                        context.SaveChanges();
                    }
                    else if (role == "Patient")
                    {
                        var patient = new Models.Patient { name = fullName };
                        patient.userId = user.Id;
                        //var patientController = System.Web.Mvc.DependencyResolver.Current.GetService<Controllers.PatientController>();
                       
                            var context = serviceProvider.GetRequiredService<HospitalContext>();
                            context.Patients.Add(patient);
                            context.SaveChanges();
                        
                    }
                }
            }
            
        }
    }
}
