using ClientSDK;
using GameData;
using UnityEngine.UIElements;

public class WidgetResourceDisplay : UIWidget
{
    private VisualElement _icon;
    private Label _text;
    private Label _name;

    public WidgetResourceDisplay(VisualElement root, IGameClient client) : base(root, client)
    {
        _icon = root.Q("ResourceIconImage").Required();
        _name = root.Q<Label>("ItemName").Required();
        _text = root.Q<Label>("ItemQtd").Required();
    }

    public void Display(ResourceSpecId id, int amount)
    {
        var resourceSpec = _client.Game.Specs.Resources[id];
        _icon.SetBackground(resourceSpec.Art);
        _name.text = resourceSpec.Name;
        _text.text = amount.ToString();

    }

    public override void Dispose()
    {
        
    }
}