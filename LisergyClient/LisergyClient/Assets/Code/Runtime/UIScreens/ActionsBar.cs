using Assets.Code.Assets.Code.Runtime.UIScreens.Layout;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Views;
using Game;
using Game.Dungeon;
using Game.Events.Bus;
using Game.Party;
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
        NONE, MOVE, GUARD, ATTACK, CHECK
    }

    public class ActionsBarSetup : UIScreenSetup
    {
        public PartyEntity Party;
        public TileEntity Tile;
        public Action<EntityAction> OnChosenAction;
    }

    public class ActionsBar : UITKScreen, IEventListener
    {
        public override UIScreen ScreenAsset => UIScreen.UnitActions;
        private Dictionary<EntityAction, Button> _buttons = new Dictionary<UI.EntityAction, Button>();
        public List<EntityAction> NoPartyActions = new EntityAction[] { EntityAction.CHECK }.ToList();

        public override void OnLoaded(VisualElement root)
        {
            //var actionsRoot = root.Q("ActionsRoot");
            //actionsRoot.style.scale = new Vector2(0.1f, 0.1f);
        }

        public override void OnClose()
        {
            //var root = Root.Q<VisualElement>("ActionsRoot");
            //root.style.scale = new Vector2(0.1f, 0.1f);
        }

        public override void OnOpen()
        {
            _buttons[EntityAction.MOVE] = Root.Q<Button>("MoveAction");
            _buttons[EntityAction.GUARD] = Root.Q<Button>("GuardAction");
            _buttons[EntityAction.ATTACK] = Root.Q<Button>("AttackAction");
            _buttons[EntityAction.CHECK] = Root.Q<Button>("CheckAction");
            foreach (var kp in _buttons)
            {
                _buttons[kp.Key].clicked += () => ClickAction(kp.Key);
            }
            var setup = GetSetup<ActionsBarSetup>();
            DisplayActionsFor(setup.Party, setup.Tile);
            //var root = Root.Q<VisualElement>("ActionsRoot");
            //root.style.scale = new Vector2(1, 1); // TODO: Animation of resize/rotate not working :( 
        }

        private List<EntityAction> EvaluateActions(PartyEntity party, TileEntity tile)
        {
            if(!party.IsInMap)
            {
                return NoPartyActions;
            }
            if (party.Tile == tile || tile == null)
            {
                return NoPartyActions;
            }
            var tileView = GameView.GetView<TileView>(tile);
            var actions = new List<EntityAction>();
            if (tileView.Building is DungeonEntity)
            {
                actions.Add(EntityAction.CHECK);
                actions.Add(EntityAction.ATTACK);
            }
            else
            {
                actions.Add(EntityAction.MOVE);
                actions.Add(EntityAction.GUARD);
            }
            return actions;
        }

        private void ClickAction(EntityAction a)
        {
            GetSetup<ActionsBarSetup>().OnChosenAction(a);
        }

        private void MoveTo(TileEntity tile)
        {
            var view = GameView.GetView(tile);
            var newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(Root.panel, view.GameObject.transform.position, Camera.main);
            Root.transform.position = newPosition;
        }

        private void DisplayActionsFor(PartyEntity party, TileEntity tile)
        {
            var actions = EvaluateActions(party, tile);
            if (actions == null || actions.Count == 0)
            {
                Log.Error($"No actions for {party} on {tile}");
                ScreenService.Close<ActionsBar>();
                return;
            }
            var visible = new List<VisualElement>();
            foreach (var kp in _buttons)
            {
                if (actions.Contains(kp.Key))
                {
                    visible.Add(_buttons[kp.Key]);
                    _buttons[kp.Key].style.display = DisplayStyle.Flex;
                }
                else
                {
                    _buttons[kp.Key].style.display = DisplayStyle.None;
                }
            }
            MoveTo(tile);
            var angle = visible.Count * 45;
            CircularLayoutGroup.ArrangeButtonsInCircle(80, angle, visible.ToArray());
        }
    }
}
