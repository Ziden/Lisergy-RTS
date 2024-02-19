using ClientSDK;
using Cysharp.Threading.Tasks;
using GameData.Specs;
using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Chat message box UI component
/// </summary>
public class ScrollableButtonWidget : VisualStruct
{
    public VisualElement Root => _root;

    public Action OnClicked;

    private VisualElement Icon;
    private Label Name;

    public ScrollableButtonWidget(VisualElement element, IGameClient client) : base(element, client)
    {
        Name = element.Q<Label>("Name").Required();
        Icon = element.Q("Icon").Required();
    }

    public async UniTaskVoid SetData(string name, ArtSpec art) 
    {
        var sprite = await _client.UnityServices().Assets.GetPrefabIcon(art);
        Icon.style.backgroundImage = new StyleBackground(sprite);
        Name.text = name;
    }

    public override void Dispose()
    {
       
    }
}