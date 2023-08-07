using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.Net.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public class Program
    {

        private DiscordSocketClient _client;


        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {

            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);


            string json = File.ReadAllText(@"C:\Users\izika\OneDrive\Рабочий стол\token.json");
            var token = Convert.ToString(JsonConvert.DeserializeObject(json));



            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Log += LogAsync;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            


            ulong guildId = 339081184476790801;
            var guilds = _client.GetGuild(guildId);


            _client.MessageUpdated += MessageUpdated;
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }


        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }



        



        public async Task Client_Ready()
        {

            ulong guildId = 339081184476790801;
            var guild = _client.GetGuild(guildId);
            

            var guildCommand = new SlashCommandBuilder()
                .WithName("list-roles")
                .WithDescription("all roles")
                .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);

           


            try
            {
                await _client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }

            

            
        }






       private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "list-roles":
                    await HandleListRoleCommand(command);
                    break;
                
            }
        }
       

        private async Task HandleListRoleCommand(SocketSlashCommand command)
        {

            var guildUser = (SocketGuildUser)command.Data.Options.First().Value;


            var roleList = string.Join("\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();


            await command.RespondAsync(embed: embedBuiler.Build());
        }

       /* public async Task CreatePrivateChannel(SocketSlashCommand command)
        {


            var guild = command.Guild;
            var categoryName = Convert.ToString(nameOfCategory);
            var textChannelName = Convert.ToString(nameOfChanels);
            var voiceChannelName = Convert.ToString(nameOfChanels);
            var roleName = Convert.ToString(nameOfRole);


            var role = await guild.CreateRoleAsync(roleName, GuildPermissions.None, null, false, false, null);
            var overwrites = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow, connect: PermValue.Allow);

            var category = await guild.CreateCategoryChannelAsync(categoryName, properties =>
            {
                properties.PermissionOverwrites = new[]
                {
                    new Overwrite(role.Id, PermissionTarget.Role, overwrites),
                    new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, OverwritePermissions.DenyAll())
                };
            });

            await guild.CreateTextChannelAsync(textChannelName, properties =>
            {
                properties.CategoryId = category.Id;
                properties.PermissionOverwrites = new[]
                {
                    new Overwrite(role.Id, PermissionTarget.Role, overwrites),
                    new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, OverwritePermissions.DenyAll())
                };
            });

            await guild.CreateVoiceChannelAsync(voiceChannelName, properties =>
            {
                properties.CategoryId = category.Id;
                properties.PermissionOverwrites = new[]
                {
                    new Overwrite(role.Id, PermissionTarget.Role, overwrites),
                    new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, OverwritePermissions.DenyAll())
                };
            });

            await command.RespondAsync($"Приватная категория '{categoryName}' создана с текстовым каналом '{textChannelName}', голосовым каналом '{voiceChannelName}' и ролью '{roleName}'.");
        } */

    }


}
