using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lockpicks
{
    public class ConfigLockEnd
    {
        public string MapName;
        public int MapX;
        public int MapY;
        [JsonIgnore]
        public bool OutLock = true;
        public ConfigLockEnd() { }
        public ConfigLockEnd(string mn, int x, int y)
        {
            MapName = mn;
            MapX = x;
            MapY = y;
        }

        public ConfigLockEnd(string str)
        {
            string[] ss = str.Split('.');
            MapName = ss[0];
            MapX = int.Parse(ss[1]);
            MapY = int.Parse(ss[2]);
        }

        public string str()
        {
            return MapName + "." + MapX + "." + MapY;
        }
    }

    class Config
    {
        public static Dictionary<string, string> OutLocks;
        public static Dictionary<string, string> InLocks;
        public static bool ready = false;
        public static void Load()
        {
            try
            {
                string filecontents = File.ReadAllText(Mod.Instance.Helper.DirectoryPath + Path.DirectorySeparatorChar + "out_locks.json");
                OutLocks = JsonConvert.DeserializeObject<Dictionary<string, string>>(filecontents);
                filecontents = File.ReadAllText(Mod.Instance.Helper.DirectoryPath + Path.DirectorySeparatorChar + "in_locks.json");
                InLocks = JsonConvert.DeserializeObject<Dictionary<string, string>>(filecontents);
                //add inverse inlocks
                foreach (var l in InLocks.Keys.ToArray())
                {
                    InLocks[InLocks[l]] = l;
                }
                ready = true;
            }
            catch (Exception e)
            {
                Mod.Instance.Monitor.Log("Failed to read lock config file: " + e.Message, StardewModdingAPI.LogLevel.Error);
            }
        }

        public static ConfigLockEnd GetMatchingOutLock(string key)
        {
            if (OutLocks.ContainsKey(key)) return new ConfigLockEnd(OutLocks[key]);
            return null;
        }

        public static ConfigLockEnd GetMatchingInLock(string key)
        {
            if (InLocks.ContainsKey(key))
            {
                var c2 = new ConfigLockEnd(InLocks[key]);
                c2.OutLock = false;
                return c2;
            }
            return null;
        }
    }
}