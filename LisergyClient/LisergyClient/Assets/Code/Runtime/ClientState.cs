using Game.Systems.Party;
using Game.Tile;

namespace Assets.Code.Assets.Code.Runtime
{
    public static class ClientState
    {
        /// <summary>
        /// Tile the player has selected and has highlight on
        /// </summary>
        public static TileEntity SelectedTile { get; private set; }

        /// <summary>
        /// Own party the player is controlling at the moment
        /// </summary>
        public static PartyEntity SelectedParty;

        static ClientState()
        {
            ClientEvents.OnSelectParty += p => SelectedParty = p;
            ClientEvents.OnClickTile += t => SelectedTile = t;
        }
    }
}
