
using Assets.Code.Entity;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Player;
using System.Collections.Generic;

namespace Assets.Code
{
    public class OtherPlayer : PlayerEntity
    {
        public OtherPlayer(GameId id) : base()
        {
            UserID = id;
        }
        public override bool Online()
        {
            return false;
        }

        public override void Send<T>(T ev) { }
    }
}
