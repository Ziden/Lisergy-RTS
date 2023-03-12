using Assets.Code.World;
using Game;
using Game.Events.Bus;
using GameData;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class ClientStrategyGame : StrategyGame
    {
        public static ClientWorld ClientWorld => _instance.GetWorld();

        private static ClientStrategyGame _instance;

        public ClientStrategyGame(GameSpec specs, GameWorld world) : base(specs, world)
        {
            if(_instance != null)
            {
                throw new Exception("Trying to instantiate two games");
            }
            _instance = this;
        }

        public ClientWorld GetWorld()
        {
           return World as ClientWorld;
        }

        public override void RegisterEventListeners()
        {
         
        }
    }
}
