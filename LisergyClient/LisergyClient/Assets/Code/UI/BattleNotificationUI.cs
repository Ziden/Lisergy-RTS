using Assets.Code;
using DG.Tweening;
using Game.Battle;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleNotificationUI : MonoBehaviour
{
    public Texture DungeonIcon;

    public RawImage BattleTypeIcon;

    public GameObject[] AttackerFaces;

    public GameObject[] DefenderFaces;

    public Button ChestButton;

    [Header("Dependencies")]
    [SerializeField] private Image outcomePanel;
    [SerializeField] private Text outcomeText;

    [Header("Settings)")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color looseColor;
    [SerializeField] private float movementY;
    [SerializeField] private Ease ease;


    private void Start()
    {
        gameObject.SetActive(false);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + movementY, transform.localPosition.z);
    }

    public void Show(BattleHeader header)
    {
        //if (header.Defender.OwnerID == null)
        //    BattleTypeIcon.texture = DungeonIcon;

        outcomePanel.gameObject.transform.localScale = Vector3.zero;

        if (IsWin(header))
        {
            outcomePanel.color = Color.green;
            outcomeText.color = Color.black;
            outcomeText.text = "WIN";
        }
        else
        {
            outcomePanel.color = Color.red;
            outcomeText.color = Color.white;
            outcomeText.text = "LOOSE";
        }

        for (var x = 0; x < 4; x++)
        {
            if (x >= header.Attacker.Units.Length)
                AttackerFaces[x].SetActive(false);
            else
                PartyUI.DrawPortrait(header.Attacker.Units[x].UnitReference, AttackerFaces[x].transform, 0.75f, 0.75f);

            if (x >= header.Defender.Units.Length)
                DefenderFaces[x].SetActive(false);
            else
                PartyUI.DrawPortrait(header.Defender.Units[x].UnitReference, DefenderFaces[x].transform, 0.75f, 0.75f);
        }
        SlideDown();
    }

    private void SlideDown()
    {
        gameObject.SetActive(true);

        var seq = DOTween.Sequence()
            .Insert(0f, transform.DOLocalMoveY(transform.localPosition.y - movementY, 1f)).SetEase(ease)
            .Insert(1f, outcomePanel.gameObject.transform.DOScale(1f, 1f)).SetEase(ease)
            .OnComplete(()=>AfterShowed());

     //   transform.DOLocalMoveY(transform.localPosition.y - movementY, 1f);

     //   Awaiter.WaitFor(TimeSpan.FromSeconds(6), () => SlideUp());
    }

    private void AfterShowed()
    {
        Awaiter.WaitFor(TimeSpan.FromSeconds(6), () => SlideUp());
    }

    private void SlideUp()
    {
        var seq = DOTween.Sequence()
            .Insert(0f, outcomePanel.gameObject.transform.DOScale(0f, .5f)).SetEase(ease)
            .Insert(.5f, transform.DOLocalMoveY(transform.localPosition.y + movementY, 1f)).SetEase(ease)
            .OnComplete(() => gameObject.SetActive(false));

        // Awaiter.WaitFor(TimeSpan.FromSeconds(1.1), () => gameObject.SetActive(false));
    }

    private bool IsWin(BattleHeader header)
    {
        return (
            header.Attacker.OwnerID == MainBehaviour.Player.UserID && header.AttackerWins
            ||
            header.Defender.OwnerID == MainBehaviour.Player.UserID && !header.AttackerWins
         );
    }
}
