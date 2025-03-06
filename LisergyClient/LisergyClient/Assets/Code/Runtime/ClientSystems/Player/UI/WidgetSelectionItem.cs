using Assets.Code.ClientSystems.Party.UI;
using GameData.Specs;
using System;
using UnityEngine.UIElements;

namespace Player.UI
{
}
/// <summary>
/// Represents a selectable item in the selection screen
/// </summary>
public class WidgetSelectionItem : WidgetElement
{
    public Action OnClicked;

    private const string USS_CARD_HIGHLIGHT = "icon-button--highlight";

    private VisualElement _icon;
    private Button _card;
    private Label _name;
    public object Data { get; private set; }

    public WidgetSelectionItem()
    {
        LoadUxmlFromResource("WidgetSelectionItem");
        _name = this.Q<Label>("Name").Required();
        _icon = this.Q("Icon").Required();
        _card = this.Q<Button>("SelectionCard").Required();
        _card.clicked += () => OnClicked?.Invoke();
    }

    public async UniTaskVoid SetData(string name, ArtSpec art, object data)
    {
        var sprite = await GameClient.UnityServices().Assets.GetSprite(art);
        _icon.style.backgroundImage = new StyleBackground(sprite);
        _name.text = name;
        Data = data;
    }

    public void SetHighlight(bool hightlight)
    {
        if (hightlight) _card.AddToClassList(USS_CARD_HIGHLIGHT);
        else _card.RemoveFromClassList(USS_CARD_HIGHLIGHT);
    }

    public new class UxmlFactory : UxmlFactory<WidgetSelectionItem, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits { }
}