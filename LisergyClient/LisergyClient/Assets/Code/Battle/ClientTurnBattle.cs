using Assets.Code.World;
using Game;
using Game.Battles;
using Game.DataTypes;
using Game.Events;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Battle
{
    public class ClientTurnBattle : TurnBattle
    {
        public ClientTurnBattle(BattleStartPacket ev): base(ev.BattleID, ev.Attacker, ev.Defender)
        {
           
        }

        public Unit FindUnit(GameId id)
        {
            var unit= Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            if(unit == null) unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            return unit.UnitReference;
        }

        public void AddToScene(Transform team1, Transform team2)
        {
            for (var x = 0; x < Attacker.Units.Length; x++)
            {
                var battleUnit = Attacker.Units[x];
                if (battleUnit == null) continue;
                var unitView = new UnitView(battleUnit.UnitReference);
                unitView.AddToScene();
                unitView.GameObject.transform.SetParent(team1.GetChild(x).transform);
                unitView.GameObject.transform.localPosition = Vector3.zero;
                unitView.GameObject.GetComponent<SpriteRenderer>().flipX = true;
                Attacker.Units[x].UnitReference = unitView.Unit;
            }

            for (var x = 0; x < Defender.Units.Length; x++)
            {
                var battleUnit = Defender.Units[x];
                if (battleUnit == null) continue;
                var unitView = new UnitView(battleUnit.UnitReference);
                var unitObj = unitView.AddToScene();
                unitObj.transform.SetParent(team2.GetChild(x).transform);
                unitObj.transform.localPosition = Vector3.zero;
                unitObj.GetComponent<SpriteRenderer>().flipX = false;
                Defender.Units[x].UnitReference = unitView.Unit;
            }
        }
    }
}
