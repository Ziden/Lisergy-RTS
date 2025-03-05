using Assets.Code.Assets.Code.UIScreens.Base;
using Game.Engine.Events.Bus;
using Game.Systems.Resources;
using GameAssets;
using Resource.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;


namespace Assets.Code.UI
{
    public class SelectScreenParam<T>
    {
        public Func<T, WidgetSelectionItem> CreateSelection;
        public List<T> Datasource;
        public Action<T> OnSelect;
        public Action<T, PreviewContainer> OnPreview;
    }

    public class PreviewContainer
    {
        public Label Time { get; private set; }
        public Label Description { get; private set; }
        public WidgetResourceDisplay [] Cost { get; private set; }

        public PreviewContainer(VisualElement Root)
        {
            Description = Root.Q<Label>("Desc").Required();
            Time = Root.Q<Label>("BuildTime").Required();
            Cost = Root.Q("Cost").Children().Where(c => c is WidgetResourceDisplay).Select(c => (WidgetResourceDisplay)c).ToArray();
        }

        public void CostFromResource(IEnumerable<ResourceStackData> resources)
        {
            var i = 0;
            foreach (var r in resources)
            {
                Cost[i].style.display = DisplayStyle.Flex;
                Cost[i].SetData(r.ResourceId, r.Amount);
                i++;
            }
            while (i < 4)
            {
                Cost[i].style.display = DisplayStyle.None;
                i++;
            }
        }
    }

    /// <summary>
    /// Screen to select something out of a list
    /// </summary>
    public class SelectScreen<T> : GameUi, IEventListener
    {
        public override UIScreen UiAsset => UIScreen.SelectScreen;

        private Button _select;
        private ScrollView _scroll;
        private SelectScreenParam<T> _param;
        private List<WidgetSelectionItem> _items = new List<WidgetSelectionItem>();
        private WidgetSelectionItem _highlighted;
        private PreviewContainer _preview;

        public override void OnOpen()
        {
            Root.Q<Button>("BackButton").Required().clicked += OnBack;
            _select = Root.Q<Button>("ActionButton").Required();
            _scroll = Root.Q<ScrollView>("Scroll").Required();
            _param = GetParameter<SelectScreenParam<T>>();
            _preview = new PreviewContainer(Root);

            foreach (var i in _param.Datasource)
            {
                var element = _param.CreateSelection(i);
                _scroll.Add(element);
                _items.Add(element);
                element.OnClicked = () =>
                {
                    _param.OnPreview.Invoke(i, _preview);
                    if (_highlighted != null) _highlighted.SetHighlight(false);
                    _highlighted = element;
                    _highlighted.SetHighlight(true);
                };
            }
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
