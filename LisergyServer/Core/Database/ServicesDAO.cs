using Game;
using LisergyServer.Core;
using System.Collections.Generic;
using System.Linq;

namespace LisergyServer.Database
{
    public class ServicesDAO
    {
        public void RegisterService(GameServiceEntity service)
        {
            using (var db = new ServicesContext())
            {
                db.Add(service);
                db.SaveChangesAsync();
            }
            Log.Info($"Registered service {service}");
        }

        public void UnregisterService(GameServiceEntity service)
        {
            using (var db = new ServicesContext())
            {
                db.Remove(service);
                db.SaveChangesAsync();
            }
        }

        public List<GameServiceEntity> GetServices(ServerType type)
        {
            using (var db = new ServicesContext())
            {
                return db.Services.Where(s => s.Type == type).ToList();
            }
        }

        public List<GameServiceEntity> GetServices()
        {
            using (var db = new ServicesContext())
            {
                return db.Services.ToList();
            }
        }
    }
}
