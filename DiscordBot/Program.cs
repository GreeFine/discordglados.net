using AREA.API;
using DiscordBot.project_recover;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace DiscordBot
{
    class Program
    {
        static private Groups group = new Groups();

        static string help(string[] args, string p_user_id)
        {
            Console.WriteLine(args);
            return "```" +
                "!help display this message\n" +
                "!join epitech mail : join the VR_POOL" +
                "```";
        }

        static void gatewayInit(Discord discord)
        {
            group.initStorage();

            discord.Gateway.add_mentioned_reaction("info", "I am a little bot made by GreeFine");
            discord.Gateway.add_mentioned_reaction("", "You asked for me ?");
            discord.Gateway.add_mentioned_reaction("poke", "Poke !");
            discord.Gateway.add_mentioned_reaction("Admin", "You are an Admin", null, true);

            discord.Gateway.add_mentioned_command("help", help);
            discord.Gateway.add_mentioned_command("createGroup", (args, p_user_id) => group.createGroup(args, p_user_id), null, true);
            discord.Gateway.add_mentioned_command("getGroups", (args, p_user_id) => group.getGroups(args, p_user_id), null, true);
            discord.Gateway.add_mentioned_command("addMember", (args, p_user_id) => group.addMember(args, p_user_id), null, true);
            discord.Gateway.add_mentioned_command("createProject", (args, p_user_id) => group.createProject(args, p_user_id), null, true);
            discord.Gateway.add_mentioned_command("checkProjects", (args, p_user_id) => group.checkProjects(args, p_user_id), null, true);
            discord.Gateway.add_mentioned_command("join", (args, p_user_id) => group.join(args, p_user_id));

            discord.Gateway.add_mentioned_command("clone", (args, p_user_id) => RepoManager.clone(args[0], args[1]), null, true);
            discord.Gateway.add_mentioned_command("cloneCmd", (args, p_user_id) => RepoManager.cloneCmd(args[0], args[1]), null, true);
        }

        static void Main(string[] args)
        {
            Discord discord = new Discord();
            gatewayInit(discord);
            Thread th = new Thread(discord.Gateway.connect);
            th.Start();

            var line = "";
            var guild = "";
            var channel = "";
            while ((line = Console.ReadLine()) != "exit")
            {
//                try
//                {
                    if (line.StartsWith("/guilde"))
                        Console.WriteLine("guilde=" + (guild = line.Replace("/guilde ", "")));
                    else if (line.StartsWith("/channel"))
                        Console.WriteLine("channel=" + (channel = line.Replace("/channel ", "")));
                    else if (line.StartsWith("/send"))
                        discord.Me.Guilds[guild].Channels_list[channel].sendMessage(line.Replace("/send", "")).addReaction("♥");
                    else if (line.StartsWith("/emojis"))
                        Console.WriteLine(discord.Me.Guilds[guild].getEmojis());
                    else if (line.StartsWith("/changename"))
                        discord.Me.ChangeName(line.Replace("/changename ", ""));
//                }
//                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }
    }
}
