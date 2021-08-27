using Assets.Code;
using Assets.Code.Battle;
using Assets.Code.World;
using DG.Tweening;
using Game;
using Game.Battles;
using Game.Battles.Actions;
using Game.BattleTactics;
using Game.Events;
using Game.Events.ClientEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleBehaviour : MonoBehaviour
{
    // Will pretend there's a server sending a battle
    public static readonly bool TEST_MODE = false;

    public Text DamageText;

    public Transform Team1;
    public Transform Team2;

    public Transform ActionPlace1;
    public Transform ActionPlace2;
    public Camera BattleCamera;

    private Camera StandardCamera;

    private ClientTurnBattle Battle;

    private List<BattleAction> Actions = new List<BattleAction>();

    private ClientUnit ActingUnit;

    private DateTime NextAction = DateTime.MaxValue;

    private float ActionDelaySeconds = 0.75f;
    private float ReadyDistanceMoved = 1f;
    private float AttackDistanceMoved = 0.2f;
    ServerBattleSimulation Test;

    void Start()
    {
        //NetworkEvents.OnBattleStart += StartBattle;
        //NetworkEvents.OnBattleAction += OnBattleAction;
      
        Debug.Log("[Battle] Battle Behaviour Waiting for Battles");
        if(!TEST_MODE)
            this.gameObject.SetActive(false);
        else
        {
            Test = new ServerBattleSimulation();
        }
    }

    public double GetNextActionDelaySeconds()
    {
        return (NextAction - DateTime.Now).TotalSeconds;
    }

    private void Update()
    {
        if(Actions.Count > 0 && DateTime.Now > NextAction)
        {
            Log.Debug($"[Battle] Executing {NextAction}");
            var action = Actions[0];
            NextAction = DateTime.Now + DrawBattleAction(action);
            action.Battle = Battle;
            Battle.ReceiveAction(action);
            Actions.RemoveAt(0);
        }
    }

    private void DrawDamage(ClientUnit unit, int damage)
    {
        DamageText.text = damage.ToString();
        var pos = Camera.main.WorldToScreenPoint(unit.GetGameObject().transform.position);
        DamageText.transform.parent.parent.position = pos;
        var hpBar = DamageText.transform.parent;
        hpBar.gameObject.SetActive(true);
        hpBar.GetComponent<HealthBar>().SetPct(80);
        DamageText.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(DamageText.transform.DOMoveY(DamageText.transform.position.y + 30, 0.2f));
        seq.Append(DamageText.transform.DOMoveY(DamageText.transform.position.y, 0.2f));
        seq.Append(DamageText.transform.DOMoveY(DamageText.transform.position.y - 10, 0.1f));
        seq.Append(DamageText.transform.DOMoveY(DamageText.transform.position.y, 0.1f));
        seq.onComplete += () => {
            Awaiter.WaitFor(TimeSpan.FromSeconds(0.4), () => {
                DamageText.gameObject.SetActive(false);
                hpBar.gameObject.SetActive(false);
            });
        };
    }

    public TimeSpan DrawBattleAction(BattleAction action)
    {
        Log.Debug($"[Battle] Handling {action} for {ActingUnit}");
        if (action is AttackAction)
        {
            var attackAction = (AttackAction)action;
            var result = (AttackActionResult)action.Result;
            var attackerID = attackAction.UnitID;
            if(attackerID == ActingUnit.Id)
            {
                // Attack Animation
                ActingUnit.Sprites.PlayAnimation(Sprite3D.ATTACK, false, 100, ActionDelaySeconds);
                ActingUnit.GetGameObject().transform.DOLocalMove(new Vector3(ReadyDistanceMoved + AttackDistanceMoved, 0, 0), ActionDelaySeconds);

                // Taking Damage
                var defender = Battle.FindUnit(attackAction.DefenderID);
                var damageAnimationDelay = ActionDelaySeconds / 2;
                Awaiter.WaitFor(TimeSpan.FromSeconds(ActionDelaySeconds / 2), () => {

                    // Swing anim
                    defender.Sprites.PlayAnimation(Sprite3D.HURT, true, 0, ActionDelaySeconds);

                    // Damage Effects
                    DrawDamage(defender, result.Damage);
                    var seq = DOTween.Sequence();
                    seq.Append(defender.GetGameObject().transform.DOMoveX(defender.GetGameObject().transform.position.x - 0.1f, damageAnimationDelay/2));
                    seq.Append(defender.GetGameObject().transform.DOMoveX(defender.GetGameObject().transform.position.x, damageAnimationDelay/2));
                    seq.onComplete += () => {
                        Awaiter.WaitFor(TimeSpan.FromMilliseconds(600), () =>
                        {
                            ActingUnit.GetGameObject().transform.DOMoveX(_originalPos.x, damageAnimationDelay);
                            ActingUnit.Sprites.PlayAnimation(Sprite3D.JUMP, true, 100, damageAnimationDelay);
                            WaitForAction();
                        });
                    };
                });
                return TimeSpan.FromSeconds(10);
            }
        }
        return TimeSpan.FromSeconds(10);
    }

    public void OnBattleAction(BattleActionPacket ev)
    {
        if (ev.BattleID != this.Battle.ID.ToString())
        {
            Log.Debug("[Battle] Received battle action for battle im not visualizing");
            return;
        }
            
        Log.Debug($"[Battle] Received {ev.Action}");
        Actions.Add(ev.Action);
    }

    private Vector3 _originalPos;
    private bool _ready = false;

    public void WaitForAction()
    {
        ActingUnit = Battle.CurrentActingUnit.UnitReference as ClientUnit;
        ActingUnit.Sprites.PlayAnimation(Sprite3D.WALK, true, 100, ActionDelaySeconds);
        var obj = ActingUnit.GetGameObject();
        _originalPos = obj.transform.position;
        obj.transform.DOLocalMove(new Vector3(ReadyDistanceMoved, 0,0), ActionDelaySeconds);
        NextAction = DateTime.Now + TimeSpan.FromSeconds(ActionDelaySeconds);
        Log.Debug($"[Battle] {ActingUnit} waiting for action to handle it in {GetNextActionDelaySeconds()} seconds");
    }

    public void StartBattle(BattleStartPacket ev)
    {
        Actions.Clear();
        this.gameObject.SetActive(true);
        UIManager.PartyUI.Hide();
        StandardCamera = Camera.main;

        StandardCamera.enabled = false;

        Battle = new ClientTurnBattle(ev);
        Battle.AddToScene(Team1, Team2);
        NextAction = DateTime.MaxValue;
        Log.Debug("[Battle] Started Battle");
        WaitForAction(); 
    }
}
