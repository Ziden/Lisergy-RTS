using Assets.Code.ClientSystems.Party.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chat.UI
{
    /// <summary>
    /// Chat message box UI component
    /// </summary>
    public class WidgetMsgbox : WidgetElement
    {
        private VisualElement Avatar;
        private Label Name;
        private Label Text;
        private bool _isMine;

        public WidgetMsgbox()
        {
            LoadUxmlFromResource("WidgetMsgbox");
            Avatar = this.Q("Avatar");
            Name = this.Q<Label>("MessageName").Required();
            Text = this.Q<Label>("MessageText").Required();
        }

        public void SetMessage(bool isMine, string name, string msg)
        {
            _isMine = isMine;
            if (isMine)
            {
                Avatar.style.unityBackgroundImageTintColor = Color.green;
            }
            else
            {
                Avatar.style.unityBackgroundImageTintColor = Color.white;
            }
            Name.text = name;
            Text.text = msg;
        }

        public new class UxmlFactory : UxmlFactory<WidgetMsgbox, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}
