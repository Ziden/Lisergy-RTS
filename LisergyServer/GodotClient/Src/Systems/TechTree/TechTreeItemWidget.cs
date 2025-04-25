using System;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Tiles.UI;

namespace LisergyGodotClient.Src.Systems.TechTree;

public partial class TechTreeItemWidget : GameUiWidget
{
	private TextureRect _border;
	private TextureButton _btn;

	private bool _inactive;
	private TextureRect _inactiveOverlay;
	[Export] public NodePath BorderPath;
	[Export] public NodePath ButtonPath;
	[Export] public NodePath InactivePath;
	[Export] public NodePath ItemWidgetPath;
	public Action<TechTreeItemWidget> OnClick;

	public ItemStackWidget ItemWidget { get; private set; }

	public object Item { get; private set; }

	private void Load()
	{
		_inactiveOverlay ??= GetNode<TextureRect>(InactivePath);
		_border ??= GetNode<TextureRect>(BorderPath);

		if (ItemWidget == null)
		{
			ItemWidget ??= GetNode<ItemStackWidget>(ItemWidgetPath);
			ItemWidget.OnClick = Clicked;
		}

		if (_btn == null)
		{
			_btn ??= GetNode<TextureButton>(ButtonPath);
			_btn.ButtonDown += Clicked;
		}
	}

	public void SetBorder(Color color)
	{
		Load();
		_border.Modulate = color;
	}

	private void Clicked()
	{
		OnClick?.Invoke(this);
	}

	public void SetActive(bool active)
	{
		Load();
		_inactive = !active;
		_inactiveOverlay.Visible = _inactive;
	}

	public void SetData(object item, ArtSpec icon, string name)
	{
		Load();
		Item = item;
		ItemWidget.SetData(icon, name, -1);
	}

	public override ArtSpec GetArt()
	{
		return AssetConfigs.WIDGET_TECH_TREE_ITEM;
	}
}
