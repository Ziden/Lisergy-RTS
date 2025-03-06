using ClientSDK;
using Game.Systems.Resources;
using Resource.UI;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class WidgetCargoComponent : VisualStruct
{
    private List<WidgetResourceDisplay> _resources = new List<WidgetResourceDisplay>();
    private Label _wt;

    public WidgetCargoComponent(VisualElement root, IGameClient client) : base(root, client)
    {
        var list = root.Q("ResourceList").Required();
        _wt = root.Q<Label>("Weight").Required();

        foreach (var c in list.Children())
        {
            _resources.Add((WidgetResourceDisplay)c);
        }
    }

    public void DisplayComponent(in CargoComponent cargo)
    {
        if (cargo.Slot1.Amount > 0)
        {
            _resources[0].Show();
            _resources[0].SetData(cargo.Slot1.ResourceId, cargo.Slot1.Amount);
        }
        else _resources[0].Hide();

        if (cargo.Slot2.Amount > 0)
        {
            _resources[1].Show();
            _resources[1].SetData(cargo.Slot2.ResourceId, cargo.Slot2.Amount);
        }
        else _resources[1].Hide();

        _wt.text = $"Weight: {cargo.CurrentWeight}/{cargo.MaxWeight}";
    }

    public override void Dispose()
    {

    }
}