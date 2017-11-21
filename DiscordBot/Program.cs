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

            while ((line = Console.ReadLine()) != "exit")
            {
            }
        }
    }
}
