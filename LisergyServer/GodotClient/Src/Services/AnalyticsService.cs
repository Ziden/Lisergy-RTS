using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisergyGodotClient.Src.Services
{
    public interface IClientAnalytics
    {
        void TrackError(Exception e);
    }

    public class GodotAnalyticsService : IClientAnalytics
    {
        public void TrackError(Exception e)
        {
            GD.PrintErr("Godot Client Exception: " + e.Message + " " + e.StackTrace);
            ClientServices.Log.Error("Godot Client Exception: "+e.Message+ " "+e.StackTrace);
        }
    }
}
