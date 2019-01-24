using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DSBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().Start(args).GetAwaiter().GetResult();

        public static CommandService _commands;
        public static DiscordSocketClient _client;
        public static Random _ran = new Random();
        private IServiceProvider _services;
        public static readonly string TEXT1 =
@"Brave Undead, welcome to the partnered **Dark Souls** Discord. Please, take your time to read the <#497782061239762957> and <#506533532202696704> channels.

To see and speak in this server, add reactions for the roles you want in the <#369216525825212416> channel:
    → Available game roles:
        DS1
        DSR
        DS2
        SotFS
        DS3
    → Available platform roles:
        Xbox 360
        Xbox One
        PS3
        PS4
        PC
        Switch

*If you come across any issue while following these steps, please contact one of our moderators*.";

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
            
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("Token.txt"));
            await _client.StartAsync();
        }

        private async Task InstallCommands()
        {
            _client.MessageReceived += HandleCommand;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;
            if (message == null || arg.Author.IsBot || !(message.Channel is IGuildChannel && (message.Channel as IGuildChannel).GuildId == DiscordIds.GetId("SERVER")))
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
            if (!reaction.UserId.Equals(_client.CurrentUser.Id) && channel.Id.Equals(DiscordIds.GetId("ROLES"))) {

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
            if (channel.Id.Equals(DiscordIds.GetId("ROLES")))
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
                if (vc.Users.Count < 1)
                {
                    await Modules.Bonfire.HandleEmptyChannel(vc);
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
