using Assets.Code.Assets.Code.UIScreens.Base;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Entities;
using Game.Systems.Battler;
using Game.Systems.Resources;
using GameAssets;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{
    public class EntityDetailsParams : IGameUiParam
    {
        public IEntity Entity;
    }

    public class ScreenEntityDetails : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.ScreenEntityDetails;

        private VisualElement _icon;
        private Label _name;

        public override void OnOpen()
        {
            var setup = GetParameter<EntityDetailsParams>();
            var e = setup.Entity;

            _icon = Root.Q("Icon").Required();
            _name = Root.Q<Label>("EntityLabel").Required();
            _name.text = e.EntityType.ToString();

            if (e.EntityType == EntityType.Party)
            {
                var leader = e.Components.Get<BattleGroupComponent>().Units.Leader;
                var leaderSpec = GameClient.Game.Specs.Units[leader.SpecId];
                _icon.SetBackground(leaderSpec.IconArt);
            }

            var groupWidget = new WidgetBattleGroup(GameClient, Root.Q("BattleGroupComponent").Required());
            if (e.Components.TryGet<BattleGroupComponent>(out var battleGroup) && battleGroup.Units.Valids > 0)
            {
                groupWidget.Show();
                groupWidget.DisplayComponent(setup.Entity, battleGroup);
            }
            else groupWidget.Hide();

            var cargoWidget = new WidgetCargoComponent(Root.Q("CargoComponent").Required(), GameClient);
            if (e.Components.TryGet<CargoComponent>(out var cargo) && cargo.CurrentWeight > 0)
            {
                cargoWidget.Show();
                cargoWidget.DisplayComponent(cargo);
            }
            else cargoWidget.Hide();
        }
    }
}
