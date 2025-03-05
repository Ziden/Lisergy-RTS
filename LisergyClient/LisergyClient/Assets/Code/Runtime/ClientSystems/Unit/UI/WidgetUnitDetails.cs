using Assets.Code.Assets.Code.UIScreens.Base;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Systems.Battler;
using Game.Tile;
using GameAssets;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{
    public class UnitDetailsSetup : IGameUiParam
    {
        public IEntity Entity;
        public Unit Unit;
    }

    public class UnitStatBar
    {
        private Label _number;
        private VisualElement _bar;

        public UnitStatBar(VisualElement e)
        {
            _bar = e.Q("GreenBar").Required();
            _number = e.Q<Label>("StatValue").Required();
        }

        public UnitStatBar SetValue(byte value)
        {
            var pct = (float)value / (float)byte.MaxValue;
            _bar.style.width = Length.Percent(pct * 100);
            _number.text = value.ToString();
            return this;
        }
    }

    // TODO: Move to Widget not Screen
    public class WidgetUnitDetails : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.ScreenUnitDetails;

        public override void OnOpen()
        {
            var setup = GetParameter<UnitDetailsSetup>();
            var spec = GameClient.Game.Specs.Units[setup.Unit.SpecId];
            Root.Q<Label>("UnitLabel").Required().text = spec.Name;
            var icon = Root.Q("Icon").Required();
            icon.SetBackground(spec.IconArt);
            Root.Q("HP").Q("GreenBar").Required().style.width = Length.Percent(setup.Unit.HpRatio * 100);
            Root.Q("HP").Q<Label>("StatValue").Required().text = $"{setup.Unit.HP} / {setup.Unit.MaxHP}";
            new UnitStatBar(Root.Q("Attack").Required()).SetValue(setup.Unit.Atk);
            new UnitStatBar(Root.Q("Defense").Required()).SetValue(setup.Unit.Def);
            new UnitStatBar(Root.Q("Matk").Required()).SetValue(setup.Unit.Matk);
            new UnitStatBar(Root.Q("Mdef").Required()).SetValue(setup.Unit.Mdef);
            new UnitStatBar(Root.Q("Speed").Required()).SetValue(setup.Unit.Speed);
            new UnitStatBar(Root.Q("Accuracy").Required()).SetValue(setup.Unit.Accuracy);
        }
    }
}
