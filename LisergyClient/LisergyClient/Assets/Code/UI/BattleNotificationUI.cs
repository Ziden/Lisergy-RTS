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

    public Color WinColor;
    public Color LooseColor;

    public GameObject[] AttackerFaces;

    public GameObject[] DefenderFaces;

    public GameObject OutcomePanel;
    public Text OutcomeText;

    public Button ChestButton;

    private bool Moving = false;
    private float y;

    public void Start()
    {
        this.gameObject.SetActive(false);
        y = transform.localPosition.y;
    }

    private bool IsWin(BattleHeader header)
    {
        return (
            header.Attacker.OwnerID == MainBehaviour.Player.UserID && header.AttackerWins
            ||
            header.Defender.OwnerID == MainBehaviour.Player.UserID && !header.AttackerWins
         );
    }

    public void Show(BattleHeader header)
    {
        //if (header.Defender.OwnerID == null)
        //    BattleTypeIcon.texture = DungeonIcon;

        if (IsWin(header))
        {
            OutcomePanel.GetComponent<Image>().color = Color.green;
            OutcomeText.text = "Win";
        }
        else
        {
            OutcomePanel.GetComponent<Image>().color = Color.red;
            OutcomeText.text = "Loose";
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
        Awaiter.WaitFor(TimeSpan.FromSeconds(3), () => SlideUp());
     }

    public void SlideUp()
    {
        this.transform.DOLocalMoveY(y + 100, 1);
        Awaiter.WaitFor(TimeSpan.FromSeconds(1.1), () => this.gameObject.SetActive(false));
    }

    public void SlideDown()
    {
        this.gameObject.SetActive(true);
        transform.localPosition = new Vector3(transform.localPosition.x, y + 100, transform.localPosition.z);
        this.transform.DOLocalMoveY(y, 1);
    }

}
