using Assets.Code.Assets.Code.UIScreens.Base;
using Chat.UI;
using ClientSDK.SDKEvents;
using Game.Engine.Events.Bus;
using Game.Network.ClientPackets;
using GameAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Assets.Code.UI
{
    public class ChatScreen : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.ChatScreen;

        private Button _sendButton;
        private TextField _input;
        private ListView _list;
        private List<ChatPacket> _chatLog = new List<ChatPacket>();
        private ChatPacket _newMessage;

        public override void OnOpen()
        {
            _input = Root.Q<TextField>("ChatInput").Required();
            _list = Root.Q<ListView>("MessageList").Required();
            _sendButton = Root.Q<Button>("SendButton").Required();
            _sendButton.clicked += OnSend;
            Root.Q<Button>("BackButton").Required().clicked += OnBack;
            GameClient.ClientEvents.On<ChatUpdateEvent>(this, OnChatUpdate);
            _chatLog = GameClient.Modules.Chat.GetFullChat().ToList();
            _list.selectionType = SelectionType.None;
            _list.makeItem += MakeItem;
            _list.bindItem += BindItem;
            _list.itemsSource = _chatLog;
            _ = ScrollToEnd();
        }

        private async UniTask ScrollToEnd()
        {
            await UniTask.NextFrame();
            _list.ScrollToItem(_chatLog.Count - 1);
        }

        private void BindItem(VisualElement e, int index)
        {
            var packet = _chatLog[index];
            var messageBox = (WidgetMsgbox)e;
            messageBox.SetMessage(packet.Owner.IsMine(), packet.Name, packet.Message);
            if (index == _chatLog.Count - 1 && _newMessage == packet)
            {
                _newMessage = null;
                e.AnimateFadeInFromLeft();
            }
        }

        private VisualElement MakeItem() => new WidgetMsgbox();

        public override void OnClose()
        {
            GameClient.ClientEvents.RemoveListener(this);
        }

        private void OnChatUpdate(ChatUpdateEvent ev)
        {
            if (ev.NewPacket == null) return;
            GameClient.Log.Debug($"Chat message received: {ev.NewPacket.Message}");
            _ = AddAnimation(ev.NewPacket);
        }

        private async UniTaskVoid AddAnimation(ChatPacket newLine)
        {
            // Scroll the whole list up for the new element
            var h = 0;
            var lastOne = _list;
            lastOne.schedule.Execute(() =>
            {
                h += 10;
                _list.style.bottom = h;
            }).Every(5).Until(() => _list.style.bottom.value.value >= _list.fixedItemHeight);
            await UniTask.WaitUntil(() => _list.style.bottom.value.value >= _list.fixedItemHeight);
            _list.style.bottom = 0;
            _chatLog.Add(newLine);
            _newMessage = newLine;
            _list.itemsSource = _chatLog;
            _list.RefreshItems();
            _list.style.marginBottom = 0;
            await ScrollToEnd();
        }

        private void OnBack()
        {
            GameClient.UnityServices().UI.Close(this);
        }

        private void OnSend()
        {
            GameClient.Modules.Chat.SendMessage(_input.text);
            _input.value = "";
            _ = SendCooldown();
        }

        private async UniTaskVoid SendCooldown()
        {
            _sendButton.SetEnabled(false);
            await UniTask.Delay(2000);
            _sendButton.SetEnabled(true);
        }
    }
}
