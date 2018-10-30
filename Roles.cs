using System;
using System.Collections.Generic;
using System.Text;

namespace DSBot {
    public class Roles {
        public struct Role {
            public ulong id;
            public string[] descriptors;

            public Role(ulong id, string[] descriptors) {
                this.id = id;
                this.descriptors = descriptors;
            }
        }

        public static Role XB360 { get; } = new Role(DiscordIds.GetId("XB360"), new string[] { "xbox 360", "xbox360", "360" });
        public static Role XB1 { get; } = new Role(DiscordIds.GetId("XB1"), new string[] { "xb1", "xbone", "xboxone", "xbox1", "xbox one", "one" });
        public static Role PS3 { get; } = new Role(DiscordIds.GetId("PS3"), new string[] { "ps3", "playstation3", "playstation 3" });
        public static Role PS4 { get; } = new Role(DiscordIds.GetId("PS4"), new string[] { "ps4", "playstation4", "playstation 4" });
        public static Role PC { get; } = new Role(DiscordIds.GetId("PC"), new string[] { "pc", "steam", "computer" });
        public static Role SWITCH { get; } = new Role(DiscordIds.GetId("SWITCH"), new string[] { "nintendo", "switch", "nintendo switch", "nintendoswitch" });

        public static Role DS1 { get; } = new Role(DiscordIds.GetId("DS1"), new string[] { "ds", "ds1", "das", "das1", "dark souls", "darksouls" });
        public static Role DSR { get; } = new Role(DiscordIds.GetId("DSR"), new string[] { "dsr", "dsre", "daremaster" });
        public static Role DS2 { get; } = new Role(DiscordIds.GetId("DS2"), new string[] { "ds2", "das2", "dark souls 2", "dark souls ii", "darksouls2", "darksoulsii" });
        public static Role SOTFS { get; } = new Role(DiscordIds.GetId("SOTFS"), new string[] { "sotfs", "scholar", "scholars", "dark souls 2 scholar of the first sin", "dark souls 2: scholar of the first sin", "dark souls ii scholar of the first sin", "dark souls ii: scholar of the first sin", "ds2softs", "ds2 sotfs", "das2sotfs" });
        public static Role DS3 { get; } = new Role(DiscordIds.GetId("DS3"), new string[] { "3", "ds3", "das3", "dark souls 3", "dark souls iii", "darksouls3", "darksoulsiii" });

        public static Role[] Platforms { get; } = new Role[] { XB360, XB1, PS3, PS4, PC, SWITCH };
        public static Role[] Games { get; } = new Role[] { DS1, DSR, DS2, SOTFS, DS3 };
        public static Role[] All { get; } = { XB360, XB1, PS3, PS4, PC, SWITCH, DS1, DSR, DS2, SOTFS, DS3 };
    }
}
