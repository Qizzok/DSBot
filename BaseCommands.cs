using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DSBot.Modules
{
    public class BaseCommands : ModuleBase
    {
        public static List<string> VOICE = new List<string>();

        [Command("test", RunMode = RunMode.Async)]
        public async Task _test()
        {
            await Console.Out.WriteLineAsync("Pass!");
            await ReplyAsync("test");
        }

        [Command("bonfire", RunMode = RunMode.Async)]
        public async Task _bonfire([Summary("Name"), Remainder()]string name = "Bonfire")
        {
            try
            {
                await Context.Message.DeleteAsync();
                VOICE.Add(name);
                IVoiceChannel channel = await Context.Guild.CreateVoiceChannelAsync(name);
                IReadOnlyCollection<ICategoryChannel> col1 = await Context.Guild.GetCategoriesAsync();
                Action<VoiceChannelProperties> action = delegate (VoiceChannelProperties x) { x.CategoryId = col1.Where(y => y.Name.ToLower().Equals("voice channels")).FirstOrDefault().Id; x.Position = (VOICE.Count.Equals(1) ? 1 : VOICE.Count - 1); };//(VOICE.Count.Equals(1) ? 1 : VOICE.Count - 1)  VOICE.Count
                await channel.ModifyAsync(action);
                //await _check(channel);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        [Command("dswelcome", RunMode = RunMode.Async)]
        public async Task dispatch()
        {
            await Context.Message.DeleteAsync();
            EmbedBuilder builder = new EmbedBuilder()
            {
                Description = Program.TEXT2,
                ThumbnailUrl = Context.Guild.IconUrl,
                Author = new EmbedAuthorBuilder() { IconUrl = Context.Guild.IconUrl, Name = $"Welcome to the partnered Dark Souls Discord!" },
                Color = new Color((byte)(Program._ran.Next(255)), (byte)(Program._ran.Next(255)), (byte)(Program._ran.Next(255)))
            }.WithCurrentTimestamp();
            IUserMessage message = await ReplyAsync(string.Empty, embed: builder.Build());
            if (Emote.TryParse("<:XboxOne:369570227337428993>", out Emote emote1))
            {
                await (message as IUserMessage).AddReactionAsync(emote1);
            }
            if (Emote.TryParse("<:PS4:368380645858344961>", out Emote emote2))
            {
                await (message as IUserMessage).AddReactionAsync(emote2);
            }
            if (Emote.TryParse("<:Xbox360:368388223354798081>", out Emote emote3))
            {
                await (message as IUserMessage).AddReactionAsync(emote3);
            }
            if (Emote.TryParse("<:PS3:368380526253441024>", out Emote emote4))
            {
                await (message as IUserMessage).AddReactionAsync(emote4);
            }
            if (Emote.TryParse("<:PC:368373647213068300>", out Emote emote5))
            {
                await (message as IUserMessage).AddReactionAsync(emote5);
            }
            if (Emote.TryParse("<:Switch:368388237367967744>", out Emote emote6))
            {
                await (message as IUserMessage).AddReactionAsync(emote6);
            }
            if (Emote.TryParse("<:ds1:368332018838405121>", out Emote emote7))
            {
                await (message as IUserMessage).AddReactionAsync(emote7);
            }
            if (Emote.TryParse("<:tilarbiedoesabetterone:429047962728005642>", out Emote emote8))
            {
                await (message as IUserMessage).AddReactionAsync(emote8);
            }
            if (Emote.TryParse("<:ds2:368334686319017985>", out Emote emote9))
            {
                await (message as IUserMessage).AddReactionAsync(emote9);
            }
            if (Emote.TryParse("<:ds2sotfs:368335262377312266>", out Emote emote10))
            {
                await (message as IUserMessage).AddReactionAsync(emote10);
            }
            if (Emote.TryParse("<:ds3:368334983250837504>", out Emote emote11))
            {
                await (message as IUserMessage).AddReactionAsync(emote11);
            }
        }

        [Command("platform", RunMode = RunMode.Async), Summary("Assigns a platform!")]
        public async Task Platform([Summary("Role")]string platform)
        {
            await TryAssignRole(Roles.Platforms, platform);
        }

        [Command("game", RunMode = RunMode.Async), Summary("Assigns a game!")]
        public async Task Game([Summary("Game")]string game)
        {
            await TryAssignRole(Roles.Games, game);
        }

        public async Task TryAssignRole(Roles.Role[] available, string requested)
        {
            try {

                foreach (Roles.Role role in available) {
                    if (role.descriptors.Contains(requested.ToLower())) {
                        await (Context.User as IGuildUser).AddRoleAsync(Context.Guild.Roles.Where(y => y.Id.Equals(role.id)).Single());
                        await ReplyAsync($"`{FirstCharToUpper(requested)} role assigned!`");
                        return;
                    }
                }

                await ReplyAsync("`Not found!`");
            } catch (Exception e) {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        [Command("username", RunMode = RunMode.Async), Summary("Assigns a username!"), RequireOwner()]
        public async Task Name([Summary("Name")]string name)
        {
            Action<SelfUserProperties> action = delegate (SelfUserProperties x) { x.Username = name; };
            await Program._client.CurrentUser.ModifyAsync(action);
        }

        private async Task _check(IVoiceChannel channel)
        {
            await Task.Delay(10000);
            while (true)
            {
                if (await channel.GetUsersAsync().Count() == 0)
                {
                    await channel.DeleteAsync(new RequestOptions() { AuditLogReason = "Empty" });
                    break;
                }
                await Task.Delay(10000);
            }
        }

        public static string FirstCharToUpper(string arg) => (String.IsNullOrEmpty(arg) ? throw new ArgumentException("ARGH!") : arg.First().ToString().ToUpper() + arg.Substring(1));
    }
}
