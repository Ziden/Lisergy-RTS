using System;
using ClientSDK;
using ClientSDK.Autoplay;
using Game.Engine;
using Game.Systems.Player;
using GameData;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Services;
using LisergyGodotClient.Src.Services.LisergyGodotClient.Src.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace LisergyGodotClient.Src;

public class ClientServices
{
	private static ClientServices _services;

	private readonly IServiceProvider _serviceProvider;

	public ClientServices(IGameObject rootNode)
	{
		var services = new ServiceCollection();

		// Lisergy base
		var log = new GodotLog("[GodotClient]");
		var camera = rootNode.GetNode().GetViewport().GetCamera3D();

		camera = camera ?? new Camera3D();
		services.AddSingleton<IClientSdk, LisergySDK>();
		services.AddSingleton(rootNode);
		services.AddSingleton(camera);
		services.AddSingleton<IGameLog>(log);
		services.AddSingleton(this);
		services.AddSingleton<AutoplayBrain>();
		
		// Bind godot specifics
	
		services.AddSingleton<IAssetService, GodotAssetService>();
		services.AddSingleton<IClientAnalytics, GodotAnalyticsService>();
		services.AddSingleton<IInputService, GodotCameraInputService>();
		services.AddSingleton<IClientStateService, ClientStateService>();
		services.AddSingleton<IUiService, GodotUiService>();
		services.AddSingleton<IMapIndicatorService, GodotMapIndicatorService>();

		_serviceProvider = services.BuildServiceProvider();

		_serviceProvider.GetService<IClientStateService>();
		_serviceProvider.GetService<IMapIndicatorService>();
		_serviceProvider.GetService<IInputService>(); // TODO: requires auto-load
		_services = this;
	}

	public static IUiService Ui => Get<IUiService>();
	public static IClientSdk ServerSdk => Get<IClientSdk>();
	public static IAssetService Assets => Get<IAssetService>();
	public static IGameLog Log => Get<IGameLog>();
	public static IClientStateService State => Get<IClientStateService>();
	public static IClientAnalytics Analytics => Get<IClientAnalytics>();
	public static IInputService Input => Get<IInputService>();
	public static IGameObject RootObject => Get<IGameObject>();
	public static PlayerModel LocalPlayer => ServerSdk.Server.Player.LocalPlayer;
	public static GameSpec GameSpecs => ServerSdk.Game.Specs;

	private GodotMapIndicatorService Indicators { get; set; }

	public static T Get<T>()
	{
		return _services._serviceProvider.GetRequiredService<T>();
	}
}
