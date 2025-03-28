using Cysharp.Threading.Tasks;
using Game.Systems.Resources;
using GameData;
using GameData.Specs;
using Godot;

namespace LisergyGodotClient.Src.Systems.Tiles.UI
{
    public partial class ResourceStackWidget : Control
    {
        [Export] public NodePath NamePath;
        [Export] public NodePath Amount;
        [Export] public NodePath Icon;

        private Label _name;
        private Label _amount;
        private TextureRect _icon;

        public override void _Ready()
        {
            _amount = GetNode<Label>(Amount);
            _name = GetNode<Label>(NamePath);
            _icon = GetNode<TextureRect>(Icon);
        }

        public void SetData(ArtSpec icon, string name, int amount)
        {
            _name.Text = name;
            _amount.Text = amount < 0 ? "" : "x"+amount.ToString();
            ClientServices.Assets.LoadGetTexture(icon).ContinueWith(tex =>
            {
                _icon.Texture = tex;
            });
        }

        public void SetData(TileSpec tile)
        {
            _name.Text = tile.Name;
            _amount.Text = "x1";
            ClientServices.Assets.LoadGetTexture(tile.Icon).ContinueWith(tex =>
            {
                _icon.Texture = tex;
            });
        }

        public void SetData(ResourceStackData resource)
        {
            var spec = ClientServices.ServerSdk.Game.Specs.Resources[resource.ResourceId];
            _name.Text = spec.Name;
            _amount.Text = $"x{resource.Amount}";
            ClientServices.Assets.LoadGetTexture(spec.Art).ContinueWith(tex =>
            {
                _icon.Texture = tex;
            });
        }

    }
}
