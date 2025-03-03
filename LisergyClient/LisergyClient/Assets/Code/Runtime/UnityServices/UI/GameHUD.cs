using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Chat.UI;
using Cysharp.Threading.Tasks;
using Game.Entities;
using Game.Engine.Events.Bus;
using Game.Systems.Movement;
using Game.Tile;
using GameAssets;
using GameData;
using Player.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    /// <summary>
    /// Main in-game hud
    /// </summary>
    public class GameHUD : GameUi, IEventListener
    {
        private WidgetEntitySelectBar _selectBar;
        private WidgetChatSummary _chatSummary;

        public override UIScreen UiAsset => UIScreen.GameHud;

        public override void OnOpen()
        {
            var service = UnityServicesContainer.Resolve<IUiService>();
            _selectBar = Root.Q<WidgetEntitySelectBar>("WidgetEntitySelectBar").Required();
            _chatSummary = Root.Q<WidgetChatSummary>("WidgetChatSummary").Required();
            _selectBar.OnTownClicked += OnTownButtonClick;
            _selectBar.OnBuildClicked += () => OpenBuildSelect().Forget();
            ClientViewState.OnCameraMove += OnCameraMove;
            ClientViewState.OnSelectTile += OnClickTile;
            ClientViewState.OnSelectEntity += OnSelectEntity;
        }

        private void OnTownButtonClick()
        {
            var center = GameClient.Modules.Player.LocalPlayer.EntityLogic.GetCenter();
            ClientViewState.SelectedEntityView = center.GetView();
        }

        private async UniTaskVoid OpenBuildSelect()
        {
            GameClient.UnityServices().UI.Open<SelectScreen<BuildingSpec>>(new SelectScreenParam<BuildingSpec>()
            {
                Datasource = GameClient.Modules.Buildings.GetBuildingsKnown().ToList(),
                OnPreview = (item, element) =>
                {
                    var constructionSpec = GameClient.Game.Specs.BuildingConstructions[item.SpecId];
                    element.Description.text = item.Description;
                    element.Time.text = $"{constructionSpec.TimeToBuildSeconds} seconds";
                    element.CostFromResource(constructionSpec.BuildingCost);
                },
                CreateSelection = b =>
                {
                    var w = new WidgetSelectionItem();
                    w.SetData(b.Name, b.Art, b).Forget();
                    return w;
                }
            });
        }

        public override void OnLoaded(VisualElement root)
        {
            base.OnLoaded(root);
        }

        public override void OnClose()
        {
            ClientViewState.OnCameraMove -= OnCameraMove;
            ClientViewState.OnSelectTile -= OnClickTile;
            GameClient.ClientEvents.RemoveListener(this);
        }

        private void OnCameraMove(Vector3 newPos)
        {
            if (newPos != Vector3.zero)
            {
                UiService.Close<WidgetUnitDetails>();
                UiService.Close<ScreenPartyActions>();
                UiService.Close<ScreenEntityDetails>();
                UiService.Close<ScreenTileDetails>();
            }
        }

        private void OnSelectEntity(IUnityEntityView entity)
        {
            UiService.Open<ScreenEntityDetails>(new EntityDetailsParams() { Entity = entity.Entity });
        }

        private void OnClickTile(TileModel tile)
        {
            UiService.Close<WidgetUnitDetails>();
            UiService.Close<ScreenPartyActions>();
            UiService.Close<ScreenEntityDetails>();
            if (ClientViewState.SelectedEntityView != null && ClientViewState.SelectedEntityView.Entity.EntityType == EntityType.Party)
            {
                UiService.Open<ScreenPartyActions>(new ActionBarParams()
                {
                    Party = ClientViewState.SelectedEntityView.Entity,
                    Tile = tile,
                    OnChosenAction = OnActionChosen
                });
            }
        }

        private void OnActionChosen(EntityAction action)
        {
            UiService.Close<ScreenPartyActions>();
            PerformActionWithSelectedParty(action);
        }

        private void PerformActionWithSelectedParty(EntityAction action)
        {
            if (ClientViewState.SelectedEntityView.Entity?.EntityType == EntityType.Party)
            {
                var tile = ClientViewState.SelectedTile;
                if (action == EntityAction.CHECK)
                {
                    var selectedTile = ClientViewState.SelectedTile;
                    if (selectedTile.Logic.Tile.GetBuildingOnTile() != null)
                    {
                        UiService.Open<ScreenEntityDetails>(new EntityDetailsParams() { Entity = selectedTile.Logic.Tile.GetBuildingOnTile() });
                    }
                    else if (selectedTile.Logic.Tile.GetEntitiesOnTile().Count > 0)
                    {
                        UiService.Open<ScreenEntityDetails>(new EntityDetailsParams() { Entity = selectedTile.Logic.Tile.GetEntitiesOnTile().First() });
                    }
                    return;
                }
                var intent = CourseIntent.Defensive;
                if (action == EntityAction.ATTACK) intent = CourseIntent.OffensiveTarget;
                else if (action == EntityAction.HARVEST) intent = CourseIntent.Harvest;
                GameClient.Modules.Actions.MoveEntity(ClientViewState.SelectedEntityView.Entity, tile, intent);
            }
        }
    }
}
