using AREA.API;
using System;
using System.Threading;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Discord discord = new Discord();
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
