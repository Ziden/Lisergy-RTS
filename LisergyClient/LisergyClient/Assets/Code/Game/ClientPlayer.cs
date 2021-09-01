
using Game;
using Game.Entity;
using Game.Events;
using System.Collections.Generic;

namespace Assets.Code
{
    public class ClientPlayer : PlayerEntity
    {

        public override bool Online()
        {
            return true;
        }

        public override void Send<T>(T ev){}
    }
}
