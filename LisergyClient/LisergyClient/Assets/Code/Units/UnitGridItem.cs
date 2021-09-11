using Assets.Code;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UnitGridItem : MonoBehaviour
{
    private Unit Unit;

    public Image Sprite;
    public Text Name;
    public Button Button;

    public void Display(Unit u)
    {
        Unit = u;
        var unitSprite = LazyLoad.GetSprite(u.Sprite, 0);
        Sprite.sprite = unitSprite;
        Name.text = Unit.Name;
    }

    public void Selection()
    {
        Button.image.color = Color.red;
    }

    public void Unselection()
    {
        Button.image.color = Color.white;
    }
}
