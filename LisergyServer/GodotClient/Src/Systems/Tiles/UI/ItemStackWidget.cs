using Game.Systems.Resources;
using GameData;
using GameData.Specs;
using Godot;
using GodotClient.Services;
using System;

namespace LisergyGodotClient.Src.Systems.Tiles.UI
{
	public partial class ItemStackWidget : GameUiWidget
	{
		[Export] public NodePath NamePath;
		[Export] public NodePath Amount;
		[Export] public NodePath Icon;
		[Export] public NodePath InvisibleButton;

		private Label _name;
		private Label _amount;
		private TextureRect _icon;
		private TextureButton _btn;

		public Action OnClick;

		public override void _Ready()
		{
			Load();
		}

		private void Load()
		{
			_amount ??= GetNode<Label>(Amount);
			_name ??= GetNode<Label>(NamePath);
			_icon ??= GetNode<TextureRect>(Icon);
			if(_btn == null)
			{
				_btn ??= GetNode<TextureButton>(InvisibleButton);
				_btn.ButtonDown += () => { OnClick?.Invoke(); };
			}
		}

		public void SetColor(Color c)
		{
			Load();
			// Get the current theme stylebox if it exists, otherwise create a new one
			var currentStyle = GetThemeStylebox("panel", "");
			StyleBoxFlat styleBox;
			if (currentStyle is StyleBoxFlat existingStyle)
			{
				// Clone the existing style to preserve other properties
				styleBox = existingStyle.Duplicate() as StyleBoxFlat;
				// Update just the border color
				styleBox.BorderColor = c;

				// Apply the modified style
				AddThemeStyleboxOverride("panel", styleBox);
			}
			else
			{
				GD.PrintErr(GetArt().ToString() + " does not have a panel stylebox");
			}
		}

		public void SetData(ArtSpec icon, string name, int amount)
		{
			Load();
			_name.Text = name;
			_amount.Text = amount < 0 ? "" : "x"+amount.ToString();
			ClientServices.Assets.LoadGetTexture(icon).Then(tex =>
			{
				_icon.Texture = tex;
			});
		}

		public void SetData(TileSpec tile)
		{
			Load();
			_name.Text = tile.Name;
			_amount.Text = "x1";
			ClientServices.Assets.LoadGetTexture(tile.Icon).Then(tex =>
			{
				_icon.Texture = tex;
			});
		}

		public void SetData(ResourceStackData resource)
		{
			Load();
			var spec = ClientServices.ServerSdk.Game.Specs.Resources[resource.ResourceId];
			_name.Text = spec.Name;
			_amount.Text = $"x{resource.Amount}";
			ClientServices.Assets.LoadGetTexture(spec.Art).Then(tex =>
			{
				_icon.Texture = tex;
			});
		}

		public override ArtSpec GetArt() => AssetConfigs.WIDGET_ITEM_STACK;
	}
}
