using Game.Systems.Player;
using Game;
using System.Collections.Generic;
using Game.Engine.DataTypes;

public class ClientPlayer : PlayerEntity
{
    public Dictionary<GameId, BaseEntity> KnownEntities = new Dictionary<GameId, BaseEntity>();

    public bool ViewBattles = true;

    public ClientPlayer(PlayerProfile profile, IGame game) : base(profile, game)
    {
    }
}