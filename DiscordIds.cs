using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSBot {
    public class DiscordIds {
        private static Dictionary<string, ulong> IDS;

        static DiscordIds()
        {
            IDS = new Dictionary<string, ulong>();
            string[] values = File.ReadAllLines("Discord_IDs.txt");
            foreach (string value in values)
            {
                string[] parts = value.Split('|', 2);
                ulong id = 0;
                ulong.TryParse(parts[1], out id);
                IDS.Add(parts[0], id);
            }
        }

        public static ulong GetId(string name)
        {
            return IDS[name];
        }
    }
}
