using Game.DataTypes;
using Game.Systems.Player;
using Game;
using System.Collections.Generic;

public class ClientPlayer : PlayerEntity
{
    public Dictionary<GameId, BaseEntity> KnownEntities = new Dictionary<GameId, BaseEntity>();

    public bool ViewBattles = true;

    public ClientPlayer(GameId id, IGame game) : base(id, game)
    {
    }
}