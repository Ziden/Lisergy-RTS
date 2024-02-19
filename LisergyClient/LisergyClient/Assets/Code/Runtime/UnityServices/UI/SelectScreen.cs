using Assets.Code.Assets.Code.UIScreens.Base;
using ClientSDK.SDKEvents;
using Cysharp.Threading.Tasks;
using Game.Events.Bus;
using Game.Network.ClientPackets;
using GameAssets;
using GameData.Specs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;


namespace Assets.Code.UI
{
    public class SelectableItem
    {
        public string Name;
        public ArtSpec Icon;
    }

    public class SelectScreenParam
    {
        public VisualTreeAsset ItemAsset;
        public List<SelectableItem> ToShow;
        public Action<int> OnSelect;
        public Action<SelectScreen> OnPreview;
    }

    public class SelectScreen : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.SelectBuildingScreen;

        private Button _select;
        private ScrollView _scroll;
        private SelectScreenParam _param;

        public override void OnOpen()
        {
            Root.Q<Button>("BackButton").Required().clicked += OnBack;
            _select = Root.Q<Button>("ActionButton").Required();
            _scroll = Root.Q<ScrollView>("Scroll").Required();
            _param = GetParameter<SelectScreenParam>();
            foreach (var i in _param.ToShow)
            {
                var item = MakeItem();
                item.SetData(i.Name, i.Icon).Forget();
                _scroll.Add(item.Root);
            }
        }

        private ScrollableButtonWidget MakeItem()
        {
            var visualTree = _param.ItemAsset;
            var template = visualTree.Instantiate();
            return new ScrollableButtonWidget(template, GameClient);
        }

        public override void OnClose()
        {
            GameClient.ClientEvents.RemoveListener(this);
        }

        private void OnBack()
        {
            GameClient.UnityServices().UI.Close(this);
        }
    }
}
