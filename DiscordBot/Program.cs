﻿using AREA.API;
using System;
using System.Threading;

namespace DiscordBot
{
    class Program
    {
        static void gatewayInit(Discord discord)
        {
            discord.Gateway.add_mentioned_reaction("help", "Sorry I can't do much for now");
            discord.Gateway.add_mentioned_reaction("info", "I am a little bot made by GreeFine nothing fancy just testing");
            discord.Gateway.add_mentioned_reaction("", "You asked for me ?");
            discord.Gateway.add_mentioned_reaction("", "Sorry I can't do much for now");
            discord.Gateway.add_mentioned_reaction("poke", "Poke !");
            discord.Gateway.add_guild_mentioned_reaction("Heavy Rain", "here", "Heavy Rain based reaction");
        }

        static void Main(string[] args)
        {
            Discord discord = new Discord();
            gatewayInit(discord);
            Thread th = new Thread(discord.Gateway.connect);
            var line = "";
            th.Start();

            discord.Me.getGuilds();

            var guild = "";
            var channel = "";
            while ((line = Console.ReadLine()) != "exit")
            {
                if (line.StartsWith("/guilde"))
                    Console.WriteLine("guilde=" + (guild = line.Replace("/guilde ", "")));
                else if (line.StartsWith("/channel"))
                    Console.WriteLine("channel=" + (channel = line.Replace("/channel ", "")));
                else if (line.StartsWith("/send"))
                    discord.Me.guilds[guild].Channels_list[channel].send(line.Replace("/send", ""));
            }
        }
    }
}
