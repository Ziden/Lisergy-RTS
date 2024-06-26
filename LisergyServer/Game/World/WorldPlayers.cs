﻿using Game.Engine.DataTypes;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.World
{
    public interface IGamePlayers
    {
        bool GetPlayer(GameId id, out PlayerEntity player);
        public void Add(PlayerEntity player);
        PlayerEntity GetPlayer(GameId id);
        public int MaxPlayers { get; }
        public int PlayerCount { get; }
        public PlayerEntity this[GameId id] => GetPlayer(id);
        public IReadOnlyCollection<PlayerEntity> AllPlayers();
    }

    /// <summary>
    /// Represents the list of players of a given world
    /// </summary>
    public class WorldPlayers : IGamePlayers
    {
        public int MaxPlayers { get; private set; }

        protected Dictionary<GameId, PlayerEntity> _players = new Dictionary<GameId, PlayerEntity>();

        public WorldPlayers(int maxPlayers)
        {
            MaxPlayers = maxPlayers;
        }

        public void Free()
        {
            _players.Clear();
        }

        public void Add(PlayerEntity p)
        {
            if (_players.ContainsKey(p.EntityId))
                return;

            if (_players.Count >= MaxPlayers)
            {
                throw new Exception("Max player limit reached");
            }
            _players[p.EntityId] = p;
        }

        public int PlayerCount { get => _players.Count; }

        public virtual bool GetPlayer(GameId id, out PlayerEntity player)
        {
            if (id == GameId.ZERO)
            {
                player = null;
                return false;
            }
            return _players.TryGetValue(id, out player);
        }

        public virtual PlayerEntity GetPlayer(GameId id)
        {
            _players.TryGetValue(id, out var player);
            return player;
        }

        public IReadOnlyCollection<PlayerEntity> AllPlayers()
        {
            return _players.Values;
        }
    }
}
