using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSBot {
    public class Emotes {
        public struct RoleEmote {
            public Emote emote;
            public Roles.Role role;

            public RoleEmote(Emote emote, Roles.Role role) {
                this.emote = emote;
                this.role = role;
            }
        }

        static Emotes() {
            try {
                RoleEmoteList = new RoleEmote[]
                {
                    new RoleEmote(Emote.Parse("<:ds1:368332018838405121>"), Roles.DS1),
                    new RoleEmote(Emote.Parse("<:ds1r:488412792207179797>"), Roles.DSR),
                    new RoleEmote(Emote.Parse("<:ds2:368334686319017985>"), Roles.DS2),
                    new RoleEmote(Emote.Parse("<:ds2sotfs:368335262377312266>"), Roles.SOTFS),
                    new RoleEmote(Emote.Parse("<:ds3:368334983250837504>"), Roles.DS3),

                    new RoleEmote(Emote.Parse("<:Xbox360:368388223354798081>"), Roles.XB360),
                    new RoleEmote(Emote.Parse("<:XboxOne:369570227337428993>"), Roles.XB1),
                    new RoleEmote(Emote.Parse("<:PS3:368380526253441024>"), Roles.PS3),
                    new RoleEmote(Emote.Parse("<:PS4:368380645858344961>"), Roles.PS4),
                    new RoleEmote(Emote.Parse("<:PC:368373647213068300>"), Roles.PC),
                    new RoleEmote(Emote.Parse("<:Switch:368388237367967744>"), Roles.SWITCH)
                };
            } catch (Exception e) {
                Console.Out.WriteLine(e);
                Console.Out.WriteLine("This is probably very bad.");
            }
        }

        public static RoleEmote[] RoleEmoteList { get; private set; } = new RoleEmote[] { };
        public static Emote Bonfire { get; private set; } = Emote.Parse("<:Bonfire:233201895609466880>");
    }
}
