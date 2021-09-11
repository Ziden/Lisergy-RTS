using Game.Events;
using Game.Events.Bus;
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
        public void JoinWorld(JoinWorldPacket p)
        {
            var player = Game.Chain.GetPlayer(p.Sender.UserID);
            if (player == null)
            {
                player = p.Sender;
                var unit = new Unit();
                unit.Name = "Lancelot";
                unit.Stats.HP = 5;
                unit.Flavour = "Famous Knight, Lancelot strives for battles and action.";
                unit.Sprite = "knight";
                player.AddUnit(unit);

                unit = new Unit();
                unit.Name = "Gawain";
                unit.Stats.HP = 5;
                unit.Flavour = "Knights of the Round Table, Gawain strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Geraint";
                unit.Stats.HP = 1;
                unit.Flavour = "Knights of the Round Table, Geraint strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Percival";
                unit.Stats.HP = 3;
                unit.Flavour = "Knights of the Round Table, Percival strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Bors";
                unit.Stats.HP = 5;
                unit.Flavour = "Knights of the Round Table, Bors strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Lamorak";
                unit.Stats.HP = 2;
                unit.Flavour = "Knights of the Round Table, Lamorak strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Tristran";
                unit.Stats.HP = 3;
                unit.Flavour = "Knights of the Round Table, Tristran strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Kay";
                unit.Stats.HP = 5;
                unit.Flavour = "Knights of the Round Table, Kay strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Gareth";
                unit.Stats.HP = 1;
                unit.Flavour = "Knights of the Round Table, Gareth strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Bedivere";
                unit.Stats.HP = 4;
                unit.Flavour = "Knights of the Round Table, Bedivere strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Gaheris";
                unit.Stats.HP = 1;
                unit.Flavour = "Knights of the Round Table, Gaheris strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Galahad";
                unit.Stats.HP = 3;
                unit.Flavour = "Knights of the Round Table, Galahad strives for battles and action.";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "Merlin";
                unit.Stats.HP = 2;
                unit.Flavour = "Famous Wizard, Merlin have lived his life in search for magical powers";
                unit.Sprite = "mage";
                player.Units.Add(unit);

                unit = new Unit();
                unit.Name = "King Arthur";
                unit.Stats.HP = 2;
                unit.Flavour = "Famous King, Arthur constructs a round table, at which only the best knights may sit";
                unit.Sprite = "knight";
                player.Units.Add(unit);

                Game.Chain.UpdatePlayer(player);
            }
            Log.Debug($"{player} joined");
            player.Send(new PlayerPacket(player));     
        }
    }
}