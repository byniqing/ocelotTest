using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using User.Api.Data;
using User.Api.Dtos;

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
                //options.UseMySQL(Configuration.GetConnectionString("Sql"));
                options.UseSqlServer(Configuration.GetConnectionString("Sql"));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddOptions();
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));

            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime lifetime,
        ILoggerFactory loggerFactory,
        IOptions<ServiceDiscoveryOptions> serviceOptions,
        IConsulClient consul)
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

            //InitTable(app);

            lifetime.ApplicationStarted.Register(() =>
            {
                RegisterService(app, lifetime, serviceOptions, consul);
            });
            //lifetime.ApplicationStopped.Register(OnStoped);
        }

        private void RegisterService(IApplicationBuilder app, IApplicationLifetime appLife,
     IOptions<ServiceDiscoveryOptions> serviceOptions,
     IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                //健康检查
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "api/HealthCheck").OriginalString
                };

                //服务注册
                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    /*
                     测试的时候用 的localhost
                     那么在DnsClient取值的时候
                     AddressList会为空，不过HostName有值
                     */
                    Address = address.Host, // "127.0.0.1",
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }
        }

        private void DeRegisterService(IApplicationBuilder app,
   IOptions<ServiceDiscoveryOptions> serviceOptions,
   IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            }
        }

        private void OnStart()
        {
            //uses default host:port which is localhost:8500
            var client = new ConsulClient();

            //健康检查
            var httpCheck = new AgentServiceCheck()
            {
                /*
                 一分钟不健康，则下线你的服务，从consul中移除掉
                 如果没下线，则会被api网关请求，说明服务器挂了，用户请求的也是500
                 */
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(10), //健康检查时间间隔，或者称为心跳间隔（定时检查服务是否健康）
                HTTP = "http://localhost:5000/api/HealthCheck" //健康检查地址
            };

            //注册
            var agentReg = new AgentServiceRegistration()
            {
                ID = "servicename:5000",
                Check = httpCheck,
                Address = "localhost",
                Name = "serviceName",
                Port = 5000
            };

            //ConfigureAwait(false); 指同步
            client.Agent.ServiceRegister(agentReg).ConfigureAwait(false);
        }
        private void OnStoped()
        {
            //uses default host:port which is localhost:8500
            var client = new ConsulClient();
            client.Agent.ServiceDeregister("servicename:5000");
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
