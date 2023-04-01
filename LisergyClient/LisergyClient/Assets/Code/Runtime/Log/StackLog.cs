
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code
{
    // THIS CAUSES MEMORY LEAK AS LOGS INSTANCES ARE NEVER GC'd BUT YO ITS GOOD FOR DEBUGGING AS HELL
    public class StackLog : IDisposable
    {
        private static List<string> Tags = new List<string>();
        private static Dictionary<string, List<string>> TagLogs = new Dictionary<string, List<string>>();

        private string _tag;

        public static int GetStackCount()
        {
            return Tags.Count;
        }

        public static void AddTag(string tag)
        {
            if (!Tags.Contains(tag))
            {
                Game.Log.Debug($"<color='green'><b>[TAG]</b> {Tabs()}{tag}</color>");
                Tags.Add(tag);
                TagLogs[tag] = new List<string>();
            }
        }

        public static void RemoveTag(string tag)
        {
            Tags.Remove(tag);
            if(TagLogs.ContainsKey(tag))
            {
                LogWindow.Window?.TagRemoved(tag, TagLogs[tag]);
                TagLogs.Remove(tag);
            }   
            else
            {
                LogWindow.Window?.TagRemoved(tag, null);
            }
        }

        public static void Debug(string s, string color = null)
        {
            if (color == null)
                Game.Log.Debug($"{Tabs()}{s}");
            else
                Game.Log.Debug($"{Tabs()}<color='{color}'>{s}</color>");

            if (Tags.Count > 0)
            {
                var current = Tags.Last();
                TagLogs[Tags.Last()].Add(s);
            }
        }

        private static string Tabs()
        {
            return new string('-', Tags.Count * 3);
        }

        public StackLog(string tag)
        {
            AddTag(tag);
         
            _tag = tag;
        }

        public void Dispose()
        {
            RemoveTag(_tag);
        }
    }
}
