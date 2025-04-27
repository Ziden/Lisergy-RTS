using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.Runtime.UIScreens;
using Game.Systems.Resources;
using GameAssets;
using Resource.UI;
using UnityEngine.UIElements;

public class FinishedHarvestingParam
{
    public ResourceStackData Resource;
    public IUnityEntityView Entity;
}

public class FinishedHarvestingNotification : Notification
{
    public override UIScreen UiAsset => UIScreen.HarvestFinishedNotification;

    private WidgetResourceDisplay _resource;

    public override void OnOpen()
    {
        base.OnOpen();
        var param = GetParameter<FinishedHarvestingParam>();
        var view = Root.Q<Button>("ViewButton").Required();
        view.clicked += () =>
        {
            ClientViewState.SelectedEntityView = param.Entity;
        };
        _resource = Root.Q<WidgetResourceDisplay>("ResourceDisplayWidget").Required();
        _resource.SetData(param.Resource.ResourceId, param.Resource.Amount);
    }
}