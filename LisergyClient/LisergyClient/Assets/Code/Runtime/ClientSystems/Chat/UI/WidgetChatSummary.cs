using Assets.Code.ClientSystems.Party.UI;
using Assets.Code.UI;
using ClientSDK;
using ClientSDK.SDKEvents;
using Cysharp.Threading.Tasks;
using Game.Events.Bus;
using GameAssets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UIElements;

namespace Chat.UI
{
    /// <summary>
    /// Small chat window to be on the main UI
    /// </summary>
    public class WidgetChatSummary : WidgetElement, IEventListener
    {
        private VisualElement _thumbnail;
        private VisualElement _container;
        private Queue<ChatUpdateEvent> _queue = new Queue<ChatUpdateEvent>();

        public WidgetChatSummary()
        {
            LoadUxmlFromResource("WidgetChatSummary");
            _thumbnail = this.Q("ChatSummary").Required();
            _container = this.Q("MessageContainer").Required();
        }

        public override void OnAddedDuringGame(IGameClient client)
        {
            client.ClientEvents.Register<ChatUpdateEvent>(this, OnChatUpdate);
            _thumbnail.RegisterCallback<PointerDownEvent>(e => client.UnityServices().UI.Open<ChatScreen>(), TrickleDown.TrickleDown);
        }

        public override void OnRemovedDuringGame(IGameClient client)
        {
            client.ClientEvents.RemoveListener(this);
        }

        private void OnChatUpdate(ChatUpdateEvent e)
        {
            if (e.NewPacket == null) return;
            var freeIndex = GetFreeIndex();
            if (freeIndex == -1) freeIndex = 2;
            var freeElement = (Label)_container.ElementAt(freeIndex);
            if (freeIndex <= 1)
            {
                freeElement.AnimateFadeIn();
                freeElement.text = $"{e.NewPacket.Name}: {e.NewPacket.Message}";
            }
            else
            {
                _queue.Enqueue(e);
                if (_queue.Count == 1) _ = ScrollAnimation(freeElement, e);
            }
        }

        private async UniTask ScrollAnimation(Label lastElement, ChatUpdateEvent e)
        {
            lastElement.text = $"{e.NewPacket.Name}: {e.NewPacket.Message}";
            var fistElement = _container.ElementAt(0);
            await fistElement.AnimateMarginTopUp(-18);
            _container.Remove(fistElement);
            fistElement.style.marginTop = 0;
            _container.Add(fistElement);
            _queue.Dequeue();
            if (_queue.TryPeek(out var next))
            {
                var freeElement = (Label)_container.ElementAt(2);
                await ScrollAnimation(freeElement, next);
            }
        }

        public int GetFreeIndex()
        {
            var idx = 0;
            foreach (var e in _container.Children())
            {
                if (e is Label label && string.IsNullOrEmpty(label.text)) return idx;
                idx++;
            }
            return -1;
        }

        public new class UxmlFactory : UxmlFactory<WidgetChatSummary, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}
