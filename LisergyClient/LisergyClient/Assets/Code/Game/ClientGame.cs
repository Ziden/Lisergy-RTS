
using Game;
using Game.BlockChain;
using Game.Events.Bus;
using GameData;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class ClientBlockchainGame : BlockchainGame
    {
        public ClientBlockchainGame() : base(new TestChain())
        {
        }

        public override void RegisterEventListeners()
        {
         
        }
    }
}
