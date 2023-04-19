using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.World;
using Game.Battle;
using Game.DataTypes;
using Game.Events.Bus;
using GameAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class BattleScreenSetup : UIScreenSetup
    {
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public Dictionary<GameId, UnitView> Units;
    }

    public class BattleScreen : UITKScreen, IEventListener
    {
        public override UIScreen ScreenAsset => UIScreen.BattleScreen;

        private IDictionary<GameId, VisualElement> UnitHealthBars = new Dictionary<GameId, VisualElement>();
        private IDictionary<GameId, UnitView> Units = new Dictionary<GameId, UnitView>();
        private IAssetService _assets;

        private void AddHealthbar(UnitView view)
        {
            _assets.GetScreen(UIScreen.HealthBar, hb =>
            {
                var tree = hb.CloneTree();
                tree.style.position = Position.Absolute;
                Root.Add(tree);
                UnitHealthBars[view.Unit.Id] = tree;
                MoveTo(tree, view);
            });
        }

        public void ToggleBar(GameId unitId, bool visible)
        {
            UnitHealthBars[unitId].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public override void OnOpen()
        {
            _assets = ServiceContainer.Resolve<IAssetService>();
            var setup = GetSetup<BattleScreenSetup>();
            this.Units = setup.Units;
            for (var i = 0; i < 4; i++)
            {
                if (i < setup.Attacker.Units.Length)
                {
                    AddHealthbar(Units[setup.Attacker.Units[i].UnitID]);
                }

                if (i < setup.Defender.Units.Length)
                {
                    AddHealthbar(Units[setup.Defender.Units[i].UnitID]);
                }
            }
        }

        private void MoveTo(VisualElement item, UnitView view)
        {
            var position = RuntimePanelUtils.CameraTransformWorldToPanel(Root.panel, view.GameObject.transform.position, Camera.main);
            item.transform.position = new Vector2(position.x - 20, position.y);
        }

        public void TakeDamage(GameId unitId, ushort damage)
        {
            var unit = Units[unitId].Unit;
            if (unit.HP - damage <= 0)
                unit.HP = 0;
            else
                unit.HP -= damage;
            PartyButton.UpdateHealth(UnitHealthBars[unitId], unit);
        }
    }
}
