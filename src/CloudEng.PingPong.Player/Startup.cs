using CloudEng.PingPong.Player.Configuration;
using CloudEng.PingPong.Player.Messaging;
using CloudEng.PingPong.Player.StateManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudEng.PingPong.Player
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PlayerConfigOptions>(Configuration.GetSection(PlayerConfigOptions.Key));
            services.AddSingleton<IGameEventManager, GameEventManager>();
            services.AddSingleton<IPlayerLoop, PlayerLoop>();
            services.AddSingleton<IPlayersLuck, PlayersLuck>();
            services.AddSingleton<IPlayerStateManager, PlayerStateManager>();
            services.AddControllers()
                .AddDapr();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCloudEvents();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
        }
    }
}