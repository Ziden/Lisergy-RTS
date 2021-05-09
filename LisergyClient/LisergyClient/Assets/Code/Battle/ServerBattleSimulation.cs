using Assets.Code;
using Game;
using Game.Battles;
using Game.Events;
using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using LisergyServer.Core;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// Can be used as a fake server to test battles
public class ServerBattleSimulation
{

    private TurnBattle Battle;

    public ServerBattleSimulation()
    {
        NetworkEvents.OnPlayerAuth += OnAuthTest;
    }

    public bool CanStartTest()
    {
        return MainBehaviour.Player != null && MainBehaviour.Player.Parties.Length > 0 && MainBehaviour.Player.Parties[0] != null && MainBehaviour.Player.Parties[0].GetUnits().ToList().Count() > 0;
    }

    public void StartTest()
    {
        var enemyUnit = new Unit(0);
        enemyUnit.SetSpecStats();
        var enemyTeam = new BattleTeam(enemyUnit);
        enemyTeam.Units[0].Controlled = false;

        var party = MainBehaviour.Player.Parties.First();
        var myClientUnit = party.GetUnits().First();
        var myUnit = new Unit(myClientUnit.SpecId);
        myUnit.SetSpecStats();
        var myTeam = new BattleTeam(myUnit);
        myTeam.Units[0].Controlled = true;

        Battle = new TurnBattle(Guid.NewGuid(), myTeam, enemyTeam);
        var ev = new BattleStartEvent()
        {
            Attacker = Battle.Attacker,
            BattleID = Battle.ID.ToString(),
            Defender = Battle.Defender
        };
        EventEmitter.CallEventFromBytes(MainBehaviour.Player, Serialization.FromEvent(ev));

        var roundActions = Battle.AutoRun.PlayOneTurn();
        foreach(var action in roundActions)
        {
            EventEmitter.CallEventFromBytes(MainBehaviour.Player, Serialization.FromEvent(new BattleActionEvent(Battle.ID.ToString(), action)));
        }
    }

    private void OnAuthTest(AuthResultEvent ev)
    {
        Awaiter.Wait(CanStartTest, StartTest);
    }
}

