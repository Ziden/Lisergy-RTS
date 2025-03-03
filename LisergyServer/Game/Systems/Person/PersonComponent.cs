using Game.Engine.ECLS;
using Game.Systems.Battler;
using System;

namespace Game.Systems.Party
{

    [Serializable]
    public struct Limb
    {

    }

    [Serializable]
    public struct Vitals
    {
        public byte Hunger;
        public byte Thirst;
    }

    [Serializable]
    public struct HumanBody
    {
        public Limb RightArm;
        public Limb LeftArm;
        public Limb RightLeg;
        public Limb LeftLeg;
        public Limb Torso;
        public Limb Head;
    }

    /// <summary>
    /// Refers to an entity that stays in the party slots of its owner
    /// </summary>
    [SyncedComponent]
    [Serializable]
    public class PersonComponent : IComponent
    {
        public Unit BattleUnit;
        public Vitals Vitals;
        public HumanBody Body;

        public override string ToString() => $"<Person body={Body} Vitals={Vitals}>";
    }
}
