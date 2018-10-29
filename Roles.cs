using System;
using System.Collections.Generic;
using System.Text;

namespace DSBot {


    public class Roles
    {
        public struct Role
            {
            public long id;
            public string[] descriptors;

            public Role(long id, string[] descriptors)
            {
                this.id = id;
                this.descriptors = descriptors;
            }
        }

        public static Role[] Platforms { get; } = new Role[]
        {
            new Role(194775666976227328, new string[] { "xb1", "xbone", "xboxone", "xbox1", "xbox one", "one" }),
            new Role(368856344188682240, new string[] { "xbox 360", "xbox360", "360" }),
            new Role(368856640671318016, new string[] { "ps3", "playstation3", "playstation 3" }),
            new Role(194775718108856320, new string[] { "ps4", "playstation4", "playstation 4" }),
            new Role(194775740162637824, new string[] { "pc", "steam", "computer" }),
            new Role(401013520637886466, new string[] { "nintendo", "switch", "nintendo switch", "nintendoswitch" })
        };

        public static Role[] Games { get; } = new Role[]
        {
            new Role(368859767084417024, new string[] { "ds", "ds1", "das", "das1", "dark souls", "darksouls" }),
            new Role(428253711857483776, new string[] { "dsr", "dsre", "daremaster" }),
            new Role(368857012106166275, new string[] { "ds2", "das2", "dark souls 2", "dark souls ii", "darksouls2", "darksoulsii" }),
            new Role(368859851293589504, new string[] { "sotfs", "scholar", "scholars", "dark souls 2 scholar of the first sin", "dark souls 2: scholar of the first sin", "dark souls ii scholar of the first sin", "dark souls ii: scholar of the first sin", "ds2softs", "ds2 sotfs", "das2sotfs" }),
            new Role(368854325516042241, new string[] { "3", "ds3", "das3", "dark souls 3", "dark souls iii", "darksouls3", "darksoulsiii" })
        };
    }
}
