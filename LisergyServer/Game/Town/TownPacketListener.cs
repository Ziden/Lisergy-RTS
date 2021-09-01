using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ClientEvents;
using Game.Events.ServerEvents;

namespace Game.Listeners
{
    public class TownPacketListener : IEventListener
    {
        private BlockchainGame Game;

        public TownPacketListener(BlockchainGame game)
        {
            this.Game = game;
            Log.Debug("Town Packet Listener Registered");
        }

        [EventMethod]
        public void EnterDungeon(EnterInfiniteDungeon p)
        {
            var party = p.Sender.GetParty(p.PartyID);

        }

        [EventMethod]
        public void CreateParty(CreatePartyPacket p)
        {
            var party = p.Sender.CreateParty(p.Units);
            p.Sender.Send(new PartyUpdatePacket(party));
        }

        [EventMethod]
        public void JoinWorld(JoinWorldPacket p)
        {
            var player = Game.Chain.GetPlayer(p.Sender.UserID);
            if(player == null)
            {
                player = p.Sender;
                var unit = new Unit();
                unit.Name = "Lancelot";
                unit.Stats.HP = 20;
                unit.Flavour = "Famous Knight, Lancelot strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Merlin";
                unit.Stats.HP = 10;
                unit.Flavour = "Famous Wizard, Merlin have lived his life in search for magical powers";
                unit.Sprite = "mage";
                player.Units.Add(unit);
                Game.Chain.UpdatePlayer(player);
            }
            Log.Debug($"{player} joined");
            player.Send(new PlayerPacket(player));
        }
    }
}