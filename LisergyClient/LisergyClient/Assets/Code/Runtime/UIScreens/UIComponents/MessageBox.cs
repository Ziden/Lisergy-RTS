
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Chat message box UI component
/// </summary>
public class MessageBox
{
    public VisualElement Root { get; private set; }
    private VisualElement Avatar;
    private Label Name;
    private Label Text;
    private bool _isMine;

    public MessageBox(VisualElement element)
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
}