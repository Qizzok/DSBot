using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DSBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().Start(args).GetAwaiter().GetResult();

        public static CommandService _commands;
        public static DiscordSocketClient _client;
        public static Random _ran = new Random();
        private IServiceProvider _services;
        public static readonly string TEXT1 = @"Brave Undead, welcome to the partnered **Dark Souls** Discord. Please, take your time to read the <#389199668111998977> channel.

To see and speak in this server, perform the following actions in the <#194771543992041472> channel:
    1) Use the `!platform` command to tag  what platform(s) you play on to gain access to matchmaking channels. e.g. `!platform PC`
    → Available platform roles:
        Switch
        One
        PS4
        360
        PS3
        PC

    2) Use the `!game` command to tag which Dark Souls games you play/own to gain access to that game's channels. e.g. `!game sotfs`
    → Available videogame roles:
        DS 
        DSR
        DS2
        SotFS
        DS3

*If you come across any issue while following these steps, please contact one of our moderators*.

Remember to check out <#389199668111998977> for more details on how this server works!";
        public static readonly string TEXT2 = @"Brave Undead, welcome to the partnered **Dark Souls** Discord. Please, take your time to read the <#389199668111998977> channel.
*If you come across any issue while following these steps, please contact one of our moderators and manually apply your roles via the* <#194771543992041472> *channel*.

To see and speak in this server, please click on the following emojis to apply roles for which:
    1) Platforms you play Dark Souls videogames on
        Switchemote - I play on Nintendo Switch
        One - I play on Xbox One 
        PS4 - I play on PlayStation 4
        360 - I play on Xbox 360
        PS3 - I play on PlayStation 3
        PC - I play on PC
        Switch - I play on Nintendo Switch

    2) Dark Souls videogames that you own
        DSemote - If you own the original Dark Souls
        DSRemote - If you own Dark Souls: Remastered
        DS2emote - If you own the DX9 versions of Dark Souls II
        SotFSemote If you own the DX11 version of Dark Souls II: Scholar of the First Sin
        DS3emote - If you own Dark Souls III";

        public async Task Start(string[] args)
        {
            await DoBotStuff();

            await Task.Delay(-1);
        }

        public async Task DoBotStuff()
        {
            _commands = new CommandService();
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
                //TotalShards = 2
            });
            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();
            _client.Log += Log;

            await InstallCommands();
            ConfigureEventHandlers();

            await _client.LoginAsync(TokenType.Bot, "REDACTED");
            await _client.StartAsync();
        }

        private async Task InstallCommands()
        {
            _client.MessageReceived += HandleCommand;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || arg.Author.IsBot)
            {
                return;
            }
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                return;
            }
            var context = new CommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
        }

        public void ConfigureEventHandlers()
        {
            _client.UserVoiceStateUpdated += _client_UserVoiceStateUpdated;
            _client.UserJoined += _client_UserJoined;
            _client.ReactionAdded += _client_ReactionAdded;
            _client.ReactionRemoved += _client_ReactionRemoved;
        }

        private async Task _client_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.UserId.Equals(_client.CurrentUser.Id) && channel.Id.Equals(369216525825212416)) {

                foreach (Emotes.RoleEmote RE in Emotes.RoleEmoteList) {
                    if (reaction.Emote.Name.Equals(RE.emote.Name)) {
                        await (reaction.User.Value as IGuildUser).AddRoleAsync((channel as IGuildChannel).Guild.Roles.Where(y => y.Id.Equals(RE.role.id)).Single());
                        break;
                    }
                }
            }
        }

        private async Task _client_ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (channel.Id.Equals(369216525825212416))
            {
                foreach (Emotes.RoleEmote RE in Emotes.RoleEmoteList)
                {
                    if (reaction.Emote.Name.Equals(RE.emote.Name))
                    {
                        await (reaction.User.Value as IGuildUser).RemoveRoleAsync((channel as IGuildChannel).Guild.Roles.Where(y => y.Id.Equals(RE.role.id)).Single());
                        break;
                    }
                }
            }
        }

        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                Description = TEXT1,
                ThumbnailUrl = (arg as SocketGuildUser).Guild.IconUrl,
                Author = new EmbedAuthorBuilder() { IconUrl = arg.GetAvatarUrl(ImageFormat.Auto, (ushort)128), Name = $"{arg.Username} joined {arg.Guild.Name}!" },
                Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
            }.WithCurrentTimestamp();
            IDMChannel y = await (arg as SocketUser).GetOrCreateDMChannelAsync();
            await y.SendMessageAsync(string.Empty, false, embed: builder.Build());
        }

        private async Task _client_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            var vc = arg2.VoiceChannel ?? arg3.VoiceChannel;
            if (!vc.Users.Contains(arg1 as SocketGuildUser)) {
                if (vc.Users.Count < 1 && Modules.BaseCommands.VOICE.Contains(vc.Name))
                {
                    await vc.DeleteAsync();
                    Modules.BaseCommands.VOICE.Remove(vc.Name);
                }
            }
        }

        private static Task Log(LogMessage arg)
        {
            var cc = Console.ForegroundColor;
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{arg.Severity,8}] {arg.Source}: {arg.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
    }
}
