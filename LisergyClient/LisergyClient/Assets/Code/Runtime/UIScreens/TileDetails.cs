using Assets.Code.Assets.Code.UIScreens.Base;
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

    public class TileDetails : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.TileDetails;

        public override void OnOpen()
        {
            var setup = GetParameter<TileDetailsParams>();
            var party = setup.Harvester;
            var tileSpec = GameClient.Game.Specs.Tiles[setup.Tile.SpecId];
            var tileResources = setup.Tile.Get<TileResourceComponent>();
            var resourceSpec = GameClient.Game.Specs.Resources[tileResources.ResourceId];
            var resourceSpotSpec = GameClient.Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];
            var img = Root.Q("ResourceIconImage").Required();
            Root.Q<Button>("HarvestButton").Required().clicked += OnClickHarvest;
            GameClient.UnityServices().Assets.GetSprite(resourceSpec.Art, sprite =>
            {
                img.style.backgroundImage = new StyleBackground(sprite);
            });
            Root.Q<Label>("ItemName").Required().text = resourceSpec.Name;
            Root.Q<Label>("ItemQtd").Required().text = $"x {tileResources.AmountResourcesLeft}";
            var cargo = party.Get<CargoComponent>();
            var resourcesAmount = tileResources.AmountResourcesLeft;
            var unitsCanCarry = (ushort)(cargo.RemainingWeight / resourceSpec.WeightPerUnit);
            if (resourcesAmount > unitsCanCarry) resourcesAmount = unitsCanCarry;
            var timeToHarvest = resourcesAmount * resourceSpotSpec.HarvestTimePerUnit;
            Root.Q<Label>("HarvestRate").Required().text = $"Rate: 1 /{resourceSpotSpec.HarvestTimePerUnit.ToReadableString()}";
            Root.Q<Label>("HarvestTime").Required().text = timeToHarvest.ToReadableString();
        }

        private void OnClickHarvest()
        {
            var setup = GetParameter<TileDetailsParams>();
            GameClient.Modules.Actions.MoveParty(setup.Harvester, setup.Tile, CourseIntent.Harvest);
        }
    }

}
