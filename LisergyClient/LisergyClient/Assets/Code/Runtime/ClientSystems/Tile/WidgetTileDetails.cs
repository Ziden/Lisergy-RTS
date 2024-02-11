using Assets.Code.Assets.Code.UIScreens.Base;
using Cysharp.Threading.Tasks;
using Game.Events.Bus;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Resources;
using Game.Tile;
using GameAssets;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{

    public class TileDetailsParams : IGameUiParam
    {
        public TileEntity Tile;
        public PartyEntity Harvester;
    }

    public class WidgetTileDetails : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.TileDetails;

        private WidgetResourceDisplay _resource;

        public override void OnOpen()
        {
            var setup = GetParameter<TileDetailsParams>();
            var party = setup.Harvester;
            var tileSpec = GameClient.Game.Specs.Tiles[setup.Tile.SpecId];
            var tileResources = setup.Tile.Get<TileResourceComponent>();
            var resourceSpec = GameClient.Game.Specs.Resources[tileResources.Resource.ResourceId];
            var resourceSpotSpec = GameClient.Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];       
            var button = Root.Q<Button>("HarvestButton");
            button.Required().clicked += OnClickHarvest;     
            var cargo = party.Get<CargoComponent>();
            var resourcesAmount = tileResources.Resource.Amount;
            var unitsCanCarry = (ushort)(cargo.RemainingWeight / resourceSpec.WeightPerUnit);
            if (resourcesAmount > unitsCanCarry) resourcesAmount = unitsCanCarry;
            var timeToHarvest = resourcesAmount * resourceSpotSpec.HarvestTimePerUnit;
            button.SetEnabled(resourcesAmount != 0);
            Root.Q<Label>("HarvestRate").Required().text = $"Rate: 1 /{resourceSpotSpec.HarvestTimePerUnit.ToReadableString()}";
            Root.Q<Label>("HarvestTime").Required().text = $"{timeToHarvest.ToReadableString()}";

            _resource = new WidgetResourceDisplay(Root.Q("Resource"), GameClient);
            _resource.Display(tileResources.Resource.ResourceId, tileResources.Resource.Amount);
        }

        private void OnClickHarvest()
        {
            var setup = GetParameter<TileDetailsParams>();
            GameClient.Modules.Actions.MoveParty(setup.Harvester, setup.Tile, CourseIntent.Harvest);
        }
    }

}
