using GameData.Specs;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src.Systems.Tiles.UI;
using System;


namespace LisergyGodotClient.Src.Systems.TechTree
{
	public partial class TechTreeItemWidget : GameUiWidget
	{
		[Export] public NodePath ItemWidgetPath;
		[Export] public NodePath ButtonPath;
		[Export] public NodePath InactivePath;
		[Export] public NodePath BorderPath;

		public ItemStackWidget ItemWidget { get; private set; }
		private TextureButton _btn;
		private TextureRect _inactiveOverlay;
		private TextureRect _border;

		public object Item { get; private set; }

		private bool _inactive;
		public Action<TechTreeItemWidget> OnClick;

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

		public override ArtSpec GetArt() => AssetConfigs.WIDGET_TECH_TREE_ITEM;
	}
}
