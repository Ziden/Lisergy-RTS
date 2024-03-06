using Assets.Code.ClientSystems.Party.UI;
using ClientSDK;
using GameData;
using UnityEngine.UIElements;

namespace Resource.UI
{
    public class WidgetResourceDisplay : WidgetElement
    {
        private VisualElement _icon;
        private Label _text;
        private Label _name;
        private GameSpec _specs;

        public WidgetResourceDisplay()
        {
            LoadUxmlFromResource("WidgetResourceDisplay");
            _icon = this.Q("ResourceIconImage").Required();
            _name = this.Q<Label>("ItemName").Required();
            _text = this.Q<Label>("ItemQtd").Required();
        }

        public override void OnAddedDuringGame(IGameClient client)
        {
            _specs = client.Game.Specs;
        }

        public void SetData(ResourceSpecId id, int amount)
        {
            var resourceSpec = _specs.Resources[id];
            _icon.SetBackground(resourceSpec.Art);
            _name.text = resourceSpec.Name;
            _text.text = amount.ToString();
        }

        public new class UxmlFactory : UxmlFactory<WidgetResourceDisplay, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}
