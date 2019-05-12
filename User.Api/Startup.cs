using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using User.Api.Data;

namespace User.Api
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
            //注入
            services.AddDbContext<DbUserContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("AppUser"));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            InitTable(app);
        }

        public void InitTable(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var user = scope.ServiceProvider.GetService<DbUserContext>();
                user.Database.Migrate(); //相当于手动执行 update-database，但必须要又migrations
                //if (!User.User.Any())
                if (user.User.Count() <= 0)
                {
                    user.User.Add(new Model.AppUser { Name = "cnblogs", Company = "博客园" });
                    user.SaveChanges();
                }
            }
        }
    }
}
