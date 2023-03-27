using Assets.Code.Views;
using Game;
using Game.Dungeon;
using Game.Movement;
using Game.Network.ClientPackets;
using Game.Party;
using Game.Pathfinder;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    public enum EntityAction
    {
        MOVE, GUARD, ATTACK, CHECK
    }

    public class ActionsUI
    {
        private PathRenderer _pathRenderer;
        private GameObject _gameObject;
        private Dictionary<EntityAction, Button> _actions = new Dictionary<EntityAction, Button>();

        public ActionsUI(GameObject obj)
        {
            _pathRenderer = new PathRenderer();
            this._gameObject = obj;
            RegisterButton("MoveButton", EntityAction.MOVE, MoveButton);
            RegisterButton("GuardButton", EntityAction.GUARD, GuardButton);
            RegisterButton("CheckButton", EntityAction.CHECK, CheckButton);
            RegisterButton("AttackButton", EntityAction.ATTACK, AttackButton);
            ClientEvents.OnClickTile += OnClickTile;
            ClientEvents.OnCameraMove += OnCameraMove;
        }

        private void RegisterButton(string name, EntityAction action, UnityAction call)
        {
            var button = this._gameObject.transform.FindDeepChild(name).GetComponent<Button>();
            button.onClick.AddListener(call);
            _actions[action] = button;
            button.gameObject.SetActive(false);
        }

        private void OnCameraMove(Vector3 oldPos, Vector3 newPos)
        {
            Hide();
        }

        private void BuildActions(params EntityAction[] actions)
        {
            foreach (var button in _actions.Values)
                button.gameObject.SetActive(false);

            var ct = actions.Count();
            if (ct > 4)
                throw new Exception("Max 4 actions for now");

            if (ct >= 1)
            {
                var action = actions[0];
                var button = _actions[action];
                button.transform.localPosition = new Vector2(0, 60);
                button.gameObject.SetActive(true);
            }
            if (ct >= 2)
            {
                var action = actions[1];
                var button = _actions[action];
                button.transform.localPosition = new Vector2(90, 0);
                button.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            _gameObject.SetActive(false);
            foreach (var button in _actions.Values)
                button.gameObject.SetActive(false);
        }

        private void OnClickTile(TileEntity tile)
        {
            Log.Debug("Actions click tile");
            if (UIManager.PartyUI.HasSelectedParty)
                DisplayActions(UIManager.PartyUI.SelectedParty, tile);
        }

        public void DisplayActions(PartyEntity party, TileEntity tile)
        {
            if (party.Tile == tile || tile == null)
            {
                Hide();
                return;
            }
            var tileView = GameView.Controller.GetView<TileView>(tile);
            var pos = Camera.main.WorldToScreenPoint(tileView.GameObject.transform.position);
            _gameObject.transform.position = pos;
            _gameObject.SetActive(true);
            var actions = new List<EntityAction>();
            if (tileView.Building is DungeonEntity)
            {
                actions.Add(EntityAction.CHECK);
                actions.Add(EntityAction.ATTACK);
            }
            else
                actions.Add(EntityAction.MOVE);
            actions.Add(EntityAction.GUARD);
            BuildActions(actions.ToArray());
        }

        private void CheckButton()
        {
            var tile = UIManager.TileUI.SelectedTile;
            var tileView = GameView.GetView<TileView>(tile);
            if (tileView.Building is DungeonEntity)
            {
                UIManager.ActionsUI.Hide();
                UIManager.DungeonsUI.Display((DungeonEntity)tileView.Building);
            }
        }

        private void MoveToSelectedTile(MovementIntent intent)
        {
            var party = UIManager.PartyUI.SelectedParty;
            TileEntity selectedTile = UIManager.TileUI.SelectedTile;
            Log.Debug($"Moving {party} to {selectedTile}");
            var map = selectedTile.Chunk.Map;
            var path = map.FindPath(party.Tile, selectedTile);
            var tilePath = path.Select(node => map.GetTile(node.X, node.Y)).ToList();
            ClientEvents.StartMovementRequest(party, tilePath);
            MainBehaviour.Networking.Send(new MoveRequestPacket()
            {
                PartyIndex = party.PartyIndex,
                Path = path.Select(p => new Position(p.X, p.Y)).ToList(),
                Intent = intent
            });
        }

        private void GuardButton() { }

        public void AttackButton()
        {
            MoveToSelectedTile(MovementIntent.Offensive);
            Hide();
        }

        public void MoveButton()
        {
            MoveToSelectedTile(MovementIntent.Defensive);
            Hide();
        }
    }
}
