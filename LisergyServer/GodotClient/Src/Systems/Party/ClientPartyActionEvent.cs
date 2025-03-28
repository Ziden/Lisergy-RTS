using ClientSDK.Data;
using Game.Engine.ECLS;
using Game.Tile;

namespace LisergyGodotClient.Src.Systems.GameHud
{
    public class ClientPartyActionEvent : IClientEvent
    {
        public EntityAction Action { get; set; }
        public IEntity TargetEntity { get; set; }
        public TileModel TargetTile { get; set; }
    }
}
