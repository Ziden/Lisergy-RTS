using Godot;
using GameData;
using GameDataTest;
using LisergyGodotClient.Src.Services;
using GodotClient;
using Cysharp.Threading.Tasks;
using Game.Engine;

namespace LisergyGodotClient.Src.Systems.Building
{
    public partial class BuildingScreen : Control
    {
        private ScrollContainer _scrollContainer;
        private GridContainer _techTreeContainer;

        private IAssetService _assets;
        private GameSpec _specs;

        public override void _Ready()
        {
            // Create the scroll container
            _assets = new GodotAssetService(new GodotGameObject(this), new GameLog("Test"));
            _scrollContainer = new ScrollContainer();
            _scrollContainer.CustomMinimumSize = new Vector2(1000, 600); // Adjust size as needed
            _scrollContainer.AnchorRight = 1.0f;
            _scrollContainer.AnchorBottom = 1.0f;
            AddChild(_scrollContainer);

            // Create the container for the tech tree
            _techTreeContainer = new GridContainer();
            _techTreeContainer.Columns = 3; // Adjust the number of columns as needed
            _scrollContainer.AddChild(_techTreeContainer);

            // Populate the tech tree
            PopulateTechTree(GetBuildingConstructionSpecs());
        }

        private void PopulateTechTree(NodeTree<BuildingSpecId> root)
        {
            PopulateTreeItem(root);
        }

        private void PopulateTreeItem(NodeTree<BuildingSpecId> node)
        {
            var spec = _specs.BuildingConstructions[node.Data];
            _assets.LoadGetTexture(spec.Icon).ContinueWith(texture =>
            {
                var textureRect = new TextureRect
                {
                    Texture = texture,
                    CustomMinimumSize = new Vector2(150, 150) // Adjust size as needed
                };
                _techTreeContainer.AddChild(textureRect);
            });
            foreach (var child in node.ChildrenNodes())
            {
                PopulateTreeItem(child);
            }
        }

        private Texture LoadTexture(string path)
        {
            var texture = (Texture)GD.Load(path);
            return texture;
        }

        private NodeTree<BuildingSpecId> GetBuildingConstructionSpecs()
        {
            _specs = TestSpecs.Generate();
            return _specs.ConstructionTechTree.Root;
        }
    }
}
