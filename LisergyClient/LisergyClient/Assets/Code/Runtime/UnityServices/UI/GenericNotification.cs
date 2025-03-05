using Assets.Code.Assets.Code.Runtime.UIScreens;
using ClientSDK.SDKEvents;
using GameAssets;
using System;
using UnityEngine.UIElements;

public class GenericNotificationParams
{
    public string Title;
    public string Message;
    public Action OnView;
}

public class GenericNotification : Notification
{
    public override UIScreen UiAsset => UIScreen.BaseNotification;

    public override void OnOpen()
    {
        base.OnOpen();
        var param = GetParameter<GenericNotificationParams>();
        var title = Root.Q<Label>("Title").Required();
        var message = Root.Q<Label>("Message").Required();
        var view = Root.Q<Button>("ViewButton").Required();

        if (string.IsNullOrEmpty(param.Message)) message.style.display = DisplayStyle.None;
        else message.text = param.Message;

        if (string.IsNullOrEmpty(param.Title)) title.style.display = DisplayStyle.None;
        else title.text = param.Title;

        if (param.OnView == null) view.style.display = DisplayStyle.None;
        else view.clicked += param.OnView;
    }
}