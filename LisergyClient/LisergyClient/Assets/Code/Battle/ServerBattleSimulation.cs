using Assets.Code;
using Game;
using Game.Battle;
using Game.Battler;
using Game.Events;
using Game.Events.ServerEvents;
using System;
using System.Linq;

// Can be used as a fake server to test battles
public class ServerBattleSimulation
{

    private TurnBattle Battle;

    public ServerBattleSimulation()
    {
        //NetworkEvents.OnPlayerAuth += OnAuthTest;
    }

    public bool CanStartTest()
    {
        return MainBehaviour.Player != null && MainBehaviour.Player.Parties.Length > 0 && MainBehaviour.Player.Parties[0] != null && MainBehaviour.Player.Parties[0].BattleGroupLogic.GetUnits().ToList().Count() > 0;
    }

    public void StartTest()
    {
        var enemyUnit = new Unit(0);
        enemyUnit.SetSpecStats();
        var enemyTeam = new BattleTeam(enemyUnit);
        enemyTeam.Units[0].Controlled = false;

        var party = MainBehaviour.Player.Parties.First();
        var myClientUnit = party.BattleGroupLogic.GetUnits().First();
        var myUnit = new Unit(myClientUnit.SpecId);
        myUnit.SetSpecStats();
        var myTeam = new BattleTeam(myUnit);
        myTeam.Units[0].Controlled = true;

        Battle = new TurnBattle(Guid.NewGuid(), myTeam, enemyTeam);
        //var ev = new BattleStartPacket(Battle.ID, myTeam, enemyTeam);
        //EventEmitter.CallEventFromBytes(MainBehaviour.Player, Serialization.FromEvent(ev));

        var roundActions = Battle.AutoRun.PlayOneTurn();
        foreach(var action in roundActions)
        {
            //EventEmitter.CallEventFromBytes(MainBehaviour.Player, Serialization.FromEvent(new BattleActionEvent(Battle.ID.ToString(), action)));
        }
    }

    private void OnAuthTest(AuthResultPacket ev)
    {
        Awaiter.Wait(CanStartTest, StartTest);
    }
}

