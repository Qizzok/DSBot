using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSBot.Modules {

    [Group("bonfire")]
    public class Bonfire : ModuleBase {

        //Mapping from user ID to channel ID
        public static Dictionary<ushort, ulong> bonfires = new Dictionary<ushort, ulong>();
        public static List<ulong> unjoined = new List<ulong>();
        
        [Command(RunMode = RunMode.Async)]
        public async Task _bonfireEmpty() {
            await ReplyAsync($"**{Context.User.Username}**, please provide a bonfire name.");
        }

        [Command(RunMode = RunMode.Async)]
        public async Task _bonfire([Summary("Name"), Remainder()]string name) {
            try {
                if (String.IsNullOrWhiteSpace(name)) {
                    await ReplyAsync($"**{Context.User.Username}**, please provide a bonfire name.");
                    return;
                }

                if (bonfires.ContainsKey(Context.User.DiscriminatorValue)) {
                    Action<VoiceChannelProperties> rename = delegate (VoiceChannelProperties vcp) {
                        vcp.Name = name;
                    };
                    IVoiceChannel userChannel = await Context.Guild.GetVoiceChannelAsync(bonfires[Context.User.DiscriminatorValue]);
                    await userChannel.ModifyAsync(rename);
                    await ReplyAsync($"**{Context.User.Username}**, your bonfire has been renamed to {name}.");
                    return;
                }
                
                IVoiceChannel newChannel = await Context.Guild.CreateVoiceChannelAsync(name);
                bonfires.Add(Context.User.DiscriminatorValue, newChannel.Id);
                IReadOnlyCollection<ICategoryChannel> categories = await Context.Guild.GetCategoriesAsync();
                Action<VoiceChannelProperties> action = delegate (VoiceChannelProperties vcp) {
                    vcp.CategoryId = categories.Where(y => y.Name.ToLower().Equals("voice channels")).First().Id;
                };
                await newChannel.ModifyAsync(action);
                await ReplyAsync($"{Emotes.Bonfire} **{Context.User.Username}**, your bonfire has been lit.");
                unjoined.Add(newChannel.Id);
                await DeleteIfStillEmpty(newChannel);
            } catch (Exception e) {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }

        [Group("limit")]
        public class LimitModule : ModuleBase {
            [Command("none", RunMode = RunMode.Async)]
            [Alias("clear", "reset")]
            [Priority(1)]
            public async Task noLimit() {
                try {
                    if (!bonfires.ContainsKey(Context.User.DiscriminatorValue)) {
                        await ReplyAsync($"**{Context.User.Username}**, you have not lit a bonfire. You can light one with `!bonfire [name]`");
                        return;
                    }

                    IVoiceChannel channel = await Context.Guild.GetVoiceChannelAsync(bonfires[Context.User.DiscriminatorValue]);
                    Action<VoiceChannelProperties> removeLimit = delegate (VoiceChannelProperties vcp) {
                        vcp.UserLimit = null;
                    };
                    await channel.ModifyAsync(removeLimit);
                    await ReplyAsync($"{Emotes.Bonfire} **{Context.User.Username}**, your bonfire has been kindled.");
                } catch (Exception e) {
                    await Console.Out.WriteLineAsync(e.ToString());
                }
            }

            [Command(RunMode = RunMode.Async)]
            [Priority(1)]
            public async Task _limit([Summary("User Count")]int count) {
                try {
                    if (!bonfires.ContainsKey(Context.User.DiscriminatorValue)) {
                        await ReplyAsync($"**{Context.User.Username}**, you have not lit a bonfire. You can light one with `!bonfire [name]`");
                        return;
                    }

                    if (count < 1 || count > 99) {
                        await ReplyAsync($"**{Context.User.Username}**, please provide a valid limit (1-99). You can remove your user limit with `!bonfire limit clear`");
                        return;
                    }

                    Action<VoiceChannelProperties> setLimit = delegate (VoiceChannelProperties vcp) {
                        vcp.UserLimit = count;
                    };

                    IVoiceChannel channel = await Context.Guild.GetVoiceChannelAsync(bonfires[Context.User.DiscriminatorValue]);
                    await channel.ModifyAsync(setLimit);
                    await ReplyAsync($"{Emotes.Bonfire} **{Context.User.Username}**, your flame has been tended.");
                } catch (Exception e) {
                    await Console.Out.WriteLineAsync(e.ToString());
                }
            }
        }

        public static async Task HandleEmptyChannel(SocketVoiceChannel vc) {
            if (bonfires.ContainsValue(vc.Id)) {
                bonfires.Remove(bonfires.Where(kvp => kvp.Value == vc.Id).First().Key);
                await vc.DeleteAsync(new RequestOptions() { AuditLogReason = "User-created channel empty" });
            }
        }

        public static void HandleUserJoin(SocketVoiceChannel vc) {
            if(unjoined.Contains(vc.Id)) {
                unjoined.Remove(vc.Id);
            }
        }

        public static async Task DeleteIfStillEmpty(IVoiceChannel vc) {
            await Task.Delay(20000);
            try {
                if (vc != null && bonfires.ContainsValue(vc.Id) && unjoined.Contains(vc.Id)) {
                    bonfires.Remove(bonfires.Where(kvp => kvp.Value == vc.Id).First().Key);
                    unjoined.Remove(vc.Id);
                    await vc.DeleteAsync(new RequestOptions() { AuditLogReason = "No users joined after 20 seconds." });
                }
            } catch (Exception e) {
                await Console.Out.WriteLineAsync(e.ToString());
            }
        }
    }
}
