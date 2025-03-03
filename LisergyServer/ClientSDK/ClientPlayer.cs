using Game;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Systems.Player;
using System.Collections.Generic;

public class ClientPlayer : PlayerModel
{
    public Dictionary<GameId, IEntity> KnownEntities = new Dictionary<GameId, IEntity>();

    public bool ViewBattles = true;

    public ClientPlayer(IGame game, IEntity playerEntity) : base(game, playerEntity)
    {
    }
}