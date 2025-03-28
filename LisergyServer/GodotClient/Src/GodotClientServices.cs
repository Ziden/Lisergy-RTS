using ClientSDK;
using Game.Engine;
using Game.Systems.Player;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Services;
using LisergyGodotClient.Src.Services.LisergyGodotClient.Src.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace LisergyGodotClient.Src
{
	public class ClientServices
	{
		private static ClientServices _services;
		public static T Get<T>() => _services._serviceProvider.GetRequiredService<T>();
        public static IUiService Ui => Get<IUiService>();
        public static IClientSDK ServerSdk => Get<IClientSDK>();
        public static IAssetService Assets => Get<IAssetService>();
        public static IGameLog Log => Get<IGameLog>();
        public static IClientStateService State => Get<IClientStateService>();
        public static IClientAnalytics Analytics => Get<IClientAnalytics>();
        public static IInputService Input => Get<IInputService>();
        public static IGameObject RootObject => Get<IGameObject>();
        public static PlayerModel LocalPlayer => ServerSdk.Server.Player.LocalPlayer;

        private IServiceProvider _serviceProvider;

		private GodotMapIndicatorService Indicators { get; set; }

        public ClientServices(IGameObject rootNode, Camera3D camera, IGameLog log)
		{
			var services = new ServiceCollection();

            // Lisergy base
			services.AddSingleton<IClientSDK, LisergySDK>();
			services.AddSingleton(rootNode);
            services.AddSingleton(camera);
            services.AddSingleton(log);
            services.AddSingleton(this);

            // Bind godot specifics
            services.AddSingleton<IAssetService, GodotAssetService>();
            services.AddSingleton<IClientAnalytics, GodotAnalyticsService>();
            services.AddSingleton<IInputService, GodotCameraInputService>();
            services.AddSingleton<IClientStateService, ClientStateService>();
            services.AddSingleton<IClientStateService, ClientStateService>();
            services.AddSingleton<IUiService, GodotUiService>();
            services.AddSingleton<IMapIndicatorService, GodotMapIndicatorService>();

            _serviceProvider = services.BuildServiceProvider();

            _serviceProvider.GetService<IMapIndicatorService>(); // TODO: requires auto-load

            _services = this;
		}
	}
}
