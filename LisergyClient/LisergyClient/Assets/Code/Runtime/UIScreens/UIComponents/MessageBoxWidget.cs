
using ClientSDK;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Chat message box UI component
/// </summary>
public class MessageBoxWidget : UIWidget
{
    public VisualElement Root => _root;
    private VisualElement Avatar;
    private Label Name;
    private Label Text;
    private bool _isMine;

    public MessageBoxWidget(VisualElement element, IGameClient client) : base(element, client)
    {
        Avatar = element.Q("Avatar");
        Name = element.Q<Label>("MessageName").Required();
        Text = element.Q<Label>("MessageText").Required();
    }

    public void SetMessage(bool isMine, string name, string msg)
    {
        _isMine = isMine;
        if(isMine)
        {
            Avatar.style.unityBackgroundImageTintColor = Color.green;
        } else
        {
            Avatar.style.unityBackgroundImageTintColor = Color.white;
        }
        Name.text = name;
        Text.text = msg;
    }

    public override void Dispose()
    {
       
    }
}