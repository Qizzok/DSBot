using Discord.Commands;
using Discord;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace DSBot.Modules
{
    public class BaseCommands : ModuleBase
    {
        private static readonly string Roles_Message =
@"Brave Undead, welcome to the partnered **Dark Souls** Discord. Please, take your time to read the <#497782061239762957> and <#506533532202696704> channels.

To see channels and speak in this server, please click on the following emoji to apply roles:
    
__Dark Souls games that you own__
    **I** - If you own the original Dark Souls
    **DSR** - If you own Dark Souls: Remastered
    **II** - If you own the DX9 versions of Dark Souls II
    **SotFS** - If you own the DX11 version of Dark Souls II: Scholar of the First Sin
    **III** - If you own Dark Souls III

__Platforms you use__
    **360** - I play on Xbox 360
    **One** - I play on Xbox One
    **PS3** - I play on PlayStation 3
    **PS4** - I play on PlayStation 4
    **PC** - I play on PC
    **Switch** - I play on Nintendo Switch

You can also manually apply your roles via the <#194771543992041472> channel. If you come across any issue while following these steps, please contact one of our moderators.";

        [Command("test", RunMode = RunMode.Async)]
        public async Task _test()
        {
            await Console.Out.WriteLineAsync("Pass!");
            await ReplyAsync("test");
        }

        [Command("dswelcome", RunMode = RunMode.Async)]
        public async Task dispatch()
        {
            await Context.Message.DeleteAsync();
            EmbedBuilder builder = new EmbedBuilder()
            {
                Description = Roles_Message,
                ThumbnailUrl = Context.Guild.IconUrl,
                Author = new EmbedAuthorBuilder() { IconUrl = Context.Guild.IconUrl, Name = $"Welcome to the partnered Dark Souls Discord!" },
                Color = new Color((byte)(Program._ran.Next(255)), (byte)(Program._ran.Next(255)), (byte)(Program._ran.Next(255)))
            }.WithCurrentTimestamp();
            IUserMessage message = await ReplyAsync(string.Empty, embed: builder.Build());

            foreach (Emotes.RoleEmote RE in Emotes.RoleEmoteList)
            {
                await (message as IUserMessage).AddReactionAsync(RE.emote);
            }
        }

        [Command("platform", RunMode = RunMode.Async), Summary("Assigns a platform!")]
        public async Task Platform([Remainder, Summary("Platform")] string platform)
        {
            await TryToggleRole(Roles.Platforms, platform);
        }

        [Command("game", RunMode = RunMode.Async), Summary("Assigns a game!")]
        public async Task Game([Remainder, Summary("Game")] string game)
        {
            await TryToggleRole(Roles.Games, game);
        }

        public async Task TryToggleRole(Roles.Role[] available, string requested)
        {
            try
            {
                foreach (Roles.Role role in available)
                {
                    if (role.descriptors.Contains(requested.ToLower()))
                    {
                        if ((Context.User as IGuildUser).RoleIds.Contains(role.id))
                        {
                            await (Context.User as IGuildUser).RemoveRoleAsync(Context.Guild.Roles.Where(y => y.Id.Equals(role.id)).Single());
                            await ReplyAsync($"`{FirstCharToUpper(requested)} role removed!`");
                        }
                        else
                        {
                            await (Context.User as IGuildUser).AddRoleAsync(Context.Guild.Roles.Where(y => y.Id.Equals(role.id)).Single());
                            await ReplyAsync($"`{FirstCharToUpper(requested)} role assigned!`");
                        }
                        return;
                    }
                }
                await ReplyAsync("`Not found!`");
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        [Command("remove", RunMode = RunMode.Async), Summary("Removes a game or platform role")]
        public async Task Remove([Remainder, Summary("Role")] string requested)
        {
            try
            {
                foreach (Roles.Role role in Roles.All)
                {
                    if (role.descriptors.Contains(requested.ToLower()))
                    {
                        await (Context.User as IGuildUser).RemoveRoleAsync(Context.Guild.Roles.Where(y => y.Id.Equals(role.id)).Single());
                        await ReplyAsync($"`{FirstCharToUpper(requested)} role removed!`");
                        return;
                    }
                }
                await ReplyAsync("`Not found!`");
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        [Command("username", RunMode = RunMode.Async), Summary("Assigns a username!"), RequireOwner()]
        public async Task Name([Summary("Name")] string name)
        {
            Action<SelfUserProperties> action = delegate (SelfUserProperties x) { x.Username = name; };
            await Program._client.CurrentUser.ModifyAsync(action);
        }

        public static string FirstCharToUpper(string arg) => (String.IsNullOrEmpty(arg) ? throw new ArgumentException("ARGH!") : arg.First().ToString().ToUpper() + arg.Substring(1));
    }
}
