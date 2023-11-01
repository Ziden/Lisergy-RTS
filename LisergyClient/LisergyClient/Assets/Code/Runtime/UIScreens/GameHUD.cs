using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Game.Events.Bus;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Tile;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    /// <summary>
    /// Main in-game hud
    /// </summary>
    public class GameHUD : GameUi, IEventListener
    {
        private EntitySelectBar _selectBar;
        private ChatSummary _chatSummary;
       
        public override UIScreen UiAsset => UIScreen.GameHud;

        public override void OnOpen()
        {
            var service = UnityServicesContainer.Resolve<IUiService>();
            _selectBar = new EntitySelectBar(GameClient, Root);
            ClientState.OnCameraMove += OnCameraMove;
            ClientState.OnSelectTile += OnClickTile;
        }

        public override void OnLoaded(VisualElement root)
        {
            base.OnLoaded(root);
        }

        public override void OnClose()
        {
            ClientState.OnCameraMove -= OnCameraMove;
            ClientState.OnSelectTile -= OnClickTile;
            GameClient.ClientEvents.RemoveListener(this);
            _selectBar.Dispose();
            _chatSummary.Dispose();
        }

        private void OnCameraMove(Vector3 newPos)
        {
            if (newPos != Vector3.zero)
            {
                UiService.Close<UnitDetails>();
                UiService.Close<ActionsBar>();
                UiService.Close<EntityDetails>();
                UiService.Close<TileDetails>();
            }
        }

        private void OnClickTile(TileEntity tile)
        {
            UiService.Close<UnitDetails>();
            UiService.Close<ActionsBar>();
            UiService.Close<EntityDetails>();
            if (ClientState.SelectedEntityView != null && ClientState.SelectedEntityView.BaseEntity is PartyEntity party)
            {
                UiService.Open<ActionsBar>(new ActionBarParams()
                {
                    Party = party,
                    Tile = tile,
                    OnChosenAction = OnActionChosen
                });
            }
        }

        private void OnActionChosen(EntityAction action)
        {
            UiService.Close<ActionsBar>();
            PerformActionWithSelectedParty(action);
        }

        private void PerformActionWithSelectedParty(EntityAction action)
        {
            if (ClientState.SelectedEntityView.BaseEntity is PartyEntity party)
            {
                var tile = ClientState.SelectedTile;
                if (action == EntityAction.CHECK)
                {
                    var selectedTile = ClientState.SelectedTile;
                    if (selectedTile.Building != null)
                    {
                        UiService.Open<EntityDetails>(new EntityDetailsParams(){ Entity = selectedTile.Building });
                    } else if(selectedTile.EntitiesIn.Count > 0)
                    {
                        UiService.Open<EntityDetails>(new EntityDetailsParams() { Entity = selectedTile.EntitiesIn[0] });
                    }
                    return;
                }
                var intent = CourseIntent.Defensive;
                if (action == EntityAction.ATTACK) intent = CourseIntent.OffensiveTarget;
                else if (action == EntityAction.HARVEST) intent = CourseIntent.Harvest;
                GameClient.Modules.Actions.MoveParty(party, tile, intent);
            }
        }
    }
}
