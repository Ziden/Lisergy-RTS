using Assets.Code.Assets.Code.Runtime.UIScreens.Layout;
using Assets.Code.Assets.Code.UIScreens.Base;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Systems.Map;
using Game.Tile;
using GameAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{
    public enum EntityAction
    {
        None, Move, Guard, Attack, Check, Harvest, Build
    }

    public class ActionBarParams : IGameUiParam
    {
        public IEntity Party;
        public TileModel Tile;
        public Action<EntityAction> OnChosenAction;
    }

    public class ScreenPartyActions : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.ScreenPartyActions;

        private Dictionary<EntityAction, Button> _buttons = new Dictionary<UI.EntityAction, Button>();
        public List<EntityAction> NoPartyActions = new EntityAction[] { EntityAction.Check }.ToList();

        public override void OnOpen()
        {
            _buttons[EntityAction.Move] = Root.Q<Button>("MoveAction");
            _buttons[EntityAction.Guard] = Root.Q<Button>("GuardAction");
            _buttons[EntityAction.Attack] = Root.Q<Button>("AttackAction");
            _buttons[EntityAction.Check] = Root.Q<Button>("CheckAction");
            _buttons[EntityAction.Harvest] = Root.Q<Button>("HarvestAction");
            _buttons[EntityAction.Build] = Root.Q<Button>("BuildAction");
            foreach (var kp in _buttons)
            {
                _buttons[kp.Key].clicked += () => ClickAction(kp.Key);
            }
            var setup = GetParameter<ActionBarParams>();
            DisplayActionsFor(setup.Party, setup.Tile);
        }

        private List<EntityAction> EvaluateActions(IEntity party, TileModel tile)
        {
            if (!party.Components.TryGet<MapPlacementComponent>(out var placed))
            {
                return NoPartyActions;
            }
            var inTile = GameClient.Game.World.GetTile(placed.Position);
            if (inTile == tile || tile == null)
            {
                return NoPartyActions;
            }
            var tileView = GameClient.Modules.Views.GetEntityView(tile.Entity);
            var actions = new List<EntityAction>();
            var buildingOnTile = tileView.Entity.Logic.Tile.GetBuildingOnTile();
            var resourcesOnTile = party.Logic.Harvesting.GetAvailableResourcesToHarvest(tile).Amount;
            if (buildingOnTile != null)
            {
                actions.Add(EntityAction.Check);
                if (!buildingOnTile.OwnerID.IsMine())
                {
                    actions.Add(EntityAction.Attack);
                    actions.Add(EntityAction.Guard);
                }
                return actions;
            }
            actions.Add(EntityAction.Move);
            if (resourcesOnTile == 0)
            {
                actions.Add(EntityAction.Build);
            } else 
            {
                actions.Add(EntityAction.Harvest);
                actions.Add(EntityAction.Check);
                actions.Add(EntityAction.Guard);
            }
            return actions;
        }

        private void ClickAction(EntityAction a)
        {
            GetParameter<ActionBarParams>().OnChosenAction(a);
        }

        private void MoveTo(TileModel tile)
        {
            var view = GameClient.Modules.Views.GetEntityView(tile.Entity);
            var newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(Root.panel, view.GameObject.Location.ToVector(), Camera.main);
            Root.transform.position = newPosition;
        }

        private void DisplayActionsFor(IEntity party, TileModel tile)
        {
            var actions = EvaluateActions(party, tile);
            if (actions == null || actions.Count == 0)
            {
                Debug.LogError($"No actions for {party} on {tile}");
                UiService.Close<ScreenPartyActions>();
                return;
            }
            var visible = new List<VisualElement>();
            foreach (var (action, button) in _buttons)
            {
                if (actions.Contains(action))
                {
                    visible.Add(button);
                    button.style.display = DisplayStyle.Flex;

                }
                else
                {
                    button.style.display = DisplayStyle.None;
                }
            }
            MoveTo(tile);
            var angle = visible.Count * 45;
            CircularLayoutGroup.ArrangeButtonsInCircle(140, angle, visible.ToArray());
        }
    }
}
