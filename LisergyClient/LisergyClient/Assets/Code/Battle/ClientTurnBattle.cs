using Assets.Code.World;
using Game.Battles;
using Game.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Battle
{
    public class ClientTurnBattle : TurnBattle
    {
        public ClientTurnBattle(BattleStartPacket ev): base(Guid.Parse(ev.BattleID), ev.Attacker, ev.Defender)
        {
           
        }

        public ClientUnit FindUnit(string id)
        {
            var unit= Attacker.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            if(unit == null) unit = Defender.Units.FirstOrDefault(u => u != null && u.UnitID == id);
            return unit.UnitReference as ClientUnit;
        }

        public void AddToScene(Transform team1, Transform team2)
        {
            for (var x = 0; x < Attacker.Units.Length; x++)
            {
                var battleUnit = Attacker.Units[x];
                if (battleUnit == null) continue;
                var clientUnit = new ClientUnit(battleUnit.UnitReference);
                clientUnit.AddToScene();
                clientUnit.GetGameObject().transform.SetParent(team1.GetChild(x).transform);
                clientUnit.GetGameObject().transform.localPosition = Vector3.zero;
                clientUnit.GetGameObject().GetComponent<SpriteRenderer>().flipX = true;
                Attacker.Units[x].UnitReference = clientUnit;
            }

            for (var x = 0; x < Defender.Units.Length; x++)
            {
                var battleUnit = Defender.Units[x];
                if (battleUnit == null) continue;
                var clientUnit = new ClientUnit(battleUnit.UnitReference);
                var unitObj = clientUnit.AddToScene();
                unitObj.transform.SetParent(team2.GetChild(x).transform);
                unitObj.transform.localPosition = Vector3.zero;
                unitObj.GetComponent<SpriteRenderer>().flipX = false;
                Defender.Units[x].UnitReference = clientUnit;
            }
        }
    }
}
