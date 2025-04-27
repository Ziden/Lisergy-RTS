using ClientSDK;
using Game.Systems.Resources;
using Resource.UI;
using System.Collections.Generic;
using Assets.Code.Code.Runtime.UnityServices.UI.Base;
using UnityEngine.UIElements;

public class WidgetCargoComponent : VisualStruct
{
    private List<WidgetResourceDisplay> _resources = new List<WidgetResourceDisplay>();
    private Label _wt;

    public WidgetCargoComponent(VisualElement root, IClientSdk client) : base(root, client)
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

        var index = 0;
        foreach (var r in _resources) r.Hide();
        foreach (var (resId, amt) in cargo.Items)
        {
            _resources[index].Show();
            _resources[index].SetData(resId, amt);
        }
        _wt.text = $"Weight: {cargo.CurrentWeight}/{cargo.MaxWeight}";
    }

    public override void Dispose()
    {

    }
}