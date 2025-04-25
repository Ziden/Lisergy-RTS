using System;
using System.Linq;
using Game.Engine.Events.Bus;
using Game.Systems.Resources;
using GameData;
using GameData.Specs;
using Godot;
using Godot.Collections;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Tiles.UI;

namespace LisergyGodotClient.Src.Systems.GameHud;

public partial class ResourcesDisplayWidget : GameUiWidget, IEventListener
{
	private Control _container;

	private Dictionary<byte, ItemStackWidget> _resourceWidgets = new();

	private ItemStackWidget _widget;
	private Control _widgetParent;
	[Export] public NodePath ResourceIcon;

	public override void OnBuild()
	{
		_widget = GetNode<ItemStackWidget>(ResourceIcon);
		_widgetParent = _widget.GetParent().GetParent<Control>().Duplicate() as Control;


		_container = this.FindFirstOfType<HBoxContainer>();
		_widgetParent.Visible = false;
		_widget.GetParent().GetParent<Control>().QueueFree();

		Update();
	}


	public void Update()
	{
		try
		{
			var resources = ClientServices.LocalPlayer.Components.Get<CargoComponent>();
			var stacks = resources.Items
				.Where(kp => ShouldDisplay(kp.Key))
				.Select(kp => new ResourceStackData(kp.Key, kp.Value))
				.ToArray();
			Display(stacks);
		}
		catch (Exception e)
		{
			ClientServices.Analytics.TrackError(e);
		}
	}

	private bool ShouldDisplay(ResourceSpecId id)
	{
		return ClientServices.GameSpecs.Resources[id].ShowInUi;
	}

	public void Display(params ResourceStackData[] resources)
	{
		foreach (var c in _container.GetChildren().ToArray())
		{
			_container.RemoveChild(c);
			c.QueueFree();
		}

		foreach (var res in resources)
		{
			var newWidget = _widgetParent.Duplicate() as Control;
			var resWidget = newWidget.FindFirstOfType<ItemStackWidget>();
			resWidget.SetData(res);
			newWidget.Visible = true;
			_container.AddChild(newWidget);
			_resourceWidgets[res.ResourceId] = resWidget;
		}
	}

	public override ArtSpec GetArt()
	{
		return AssetConfigs.WIDGET_RESOURCES_AMOUNT;
	}
}
