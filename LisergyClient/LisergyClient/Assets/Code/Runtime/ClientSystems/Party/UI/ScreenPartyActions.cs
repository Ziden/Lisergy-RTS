using Assets.Code.Assets.Code.Runtime.UIScreens.Layout;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Views;
using Game.Events.Bus;
using Game.Systems.Dungeon;
using Game.Systems.Party;
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
        NONE, MOVE, GUARD, ATTACK, CHECK, HARVEST
    }

    public class ActionBarParams : IGameUiParam
    {
        public PartyEntity Party;
        public TileEntity Tile;
        public Action<EntityAction> OnChosenAction;
    }

    public class ScreenPartyActions : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.ScreenPartyActions;

        private Dictionary<EntityAction, Button> _buttons = new Dictionary<UI.EntityAction, Button>();
        public List<EntityAction> NoPartyActions = new EntityAction[] { EntityAction.CHECK }.ToList();

        public override void OnOpen()
        {
            _buttons[EntityAction.MOVE] = Root.Q<Button>("MoveAction");
            _buttons[EntityAction.GUARD] = Root.Q<Button>("GuardAction");
            _buttons[EntityAction.ATTACK] = Root.Q<Button>("AttackAction");
            _buttons[EntityAction.CHECK] = Root.Q<Button>("CheckAction");
            _buttons[EntityAction.HARVEST] = Root.Q<Button>("HarvestAction");
            foreach (var kp in _buttons)
            {
                _buttons[kp.Key].clicked += () => ClickAction(kp.Key);
            }
            var setup = GetParameter<ActionBarParams>();
            DisplayActionsFor(setup.Party, setup.Tile);
        }

        private List<EntityAction> EvaluateActions(PartyEntity party, TileEntity tile)
        {
            if(party.Tile == null)
            {
                return NoPartyActions;
            }
            if (party.Tile == tile || tile == null)
            {
                return NoPartyActions;
            }
            var tileView = GameClient.Modules.Views.GetView<TileView>(tile);
            var actions = new List<EntityAction>();
            if (tileView.Entity.Building is DungeonEntity)
            {
                actions.Add(EntityAction.CHECK);
                if(GameClient.Modules.Player.LocalPlayer.Buildings.Any()) actions.Add(EntityAction.ATTACK);
                return actions; 
            }
  
            actions.Add(EntityAction.MOVE);
            if(party.EntityLogic.Harvesting.GetAvailableResourcesToHarvest(tile).Amount > 0)
            {
                actions.Add(EntityAction.HARVEST);
            }        
            return actions;
        }

        private void ClickAction(EntityAction a)
        {
            GetParameter<ActionBarParams>().OnChosenAction(a);
        }

        private void MoveTo(TileEntity tile)
        {
            var view = GameClient.Modules.Views.GetView<TileView>(tile);
            var newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(Root.panel, view.GameObject.transform.position, Camera.main);
            Root.transform.position = newPosition;
        }

        private void DisplayActionsFor(PartyEntity party, TileEntity tile)
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
