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

            foreach (Emotes.RoleEmote RE in Emotes.RoleEmoteList)
            {
                await (message as IUserMessage).AddReactionAsync(RE.emote);
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
