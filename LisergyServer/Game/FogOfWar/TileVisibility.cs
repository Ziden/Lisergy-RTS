﻿using Game.ECS;
using Game.Player;
using Game.Tile;
using System.Collections.Generic;

namespace Game.FogOfWar
{
    public class TileVisibility : IComponent
    {
        public HashSet<PlayerEntity> PlayersViewing = new HashSet<PlayerEntity>();
        public HashSet<WorldEntity> EntitiesViewing = new HashSet<WorldEntity>();

        static TileVisibility()
        {
            SystemRegistry<TileVisibility, TileEntity>.AddSystem(new TileVisibilitySystem());
        }

    }
}