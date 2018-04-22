using DiscordBot.Features;
using DiscordBot.project_recover;
using System;
using System.Threading;

namespace DiscordBot
{
    class Program
    {
        static private Groups group = new Groups();

        static string help(string[] args, Message p_message)
        {
            Console.WriteLine(args);
            return "```" +
                "@me help display this message\n" +
                "@me join epitech mail : join the VR_POOL" +
                "@me steamid name : get you steamid" +
                "@me csgostats SteamId : get you csogstats" +
                "@me display admins" +
                "@me cleanText [me or number to delete]" +
                "```";
        }

        static void steamInit(Discord discord)
        {
            discord.Gateway.add_mentioned_command("steamid", (args, p_message) => csgo.stats.steamId(args));
            discord.Gateway.add_mentioned_command("csgostats", (args, p_message) => csgo.stats.get(args));
        }

        static void groupsInit(Discord discord)
        {
            discord.Gateway.add_mentioned_command("createGroup", (args, p_message) => group.createGroup(args, p_message), null, true);
            discord.Gateway.add_mentioned_command("getGroups", (args, p_message) => group.getGroups(args, p_message), null, true);
            discord.Gateway.add_mentioned_command("addMember", (args, p_message) => group.addMember(args, p_message), null, true);
            discord.Gateway.add_mentioned_command("createProject", (args, p_message) => group.createProject(args, p_message), null, true);
            discord.Gateway.add_mentioned_command("checkProjects", (args, p_message) => group.checkProjects(args, p_message), null, true);
            discord.Gateway.add_mentioned_command("join", (args, p_message) => group.join(args, p_message));

            discord.Gateway.add_mentioned_command("clone", (args, p_message) => RepoManager.clone(args[0], args[1]), null, true);
            discord.Gateway.add_mentioned_command("cloneCmd", (args, p_message) => RepoManager.cloneCmd(args[0], args[1]), null, true);
        }

        static void basicInit(Discord discord)
        {
            discord.Gateway.add_mentioned_reaction("info", "I am a little bot made by GreeFine");
            discord.Gateway.add_mentioned_reaction("", "You asked for me ?");
            discord.Gateway.add_mentioned_reaction("poke", "Poke !");
            discord.Gateway.add_mentioned_reaction("Admin", "You are an Admin", null, true);
            discord.Gateway.add_mentioned_command("displayAdmins", (args, p_message) => Priviledge.getAdmins());
            discord.Gateway.add_mentioned_command("addAdmin", (args, p_message) => Priviledge.addAdmin(args), null, true);

            discord.Gateway.add_mentioned_command("cleanText", (args, p_message) => Cleaner.channel_remove_text_message(args, p_message), null, true);
            discord.Gateway.add_mentioned_command("pipo", (args, p_message) => Pipotronic.getPipo(), null, true);
            discord.Gateway.add_mentioned_command("someone", (args, p_message) => Someone.find(p_message), null, true);
            discord.Gateway.add_mentioned_command("pipoGame", (args, p_message) => Pipotronic.pipoGame(p_message), null, true);

            discord.Gateway.add_mentioned_command("help", help);
        }

        static void Main(string[] args)
        {
            Discord discord = new Discord();

            basicInit(discord);
            groupsInit(discord);
            steamInit(discord);

            Thread th = new Thread(discord.Gateway.connect);
            th.Start();

            var line = "";
            var guild = "";
            var channel = "";
            while ((line = Console.ReadLine()) != "exit")
            {
                try
                {
                    if (line.StartsWith("/guilde"))
                        Console.WriteLine("guilde=" + (guild = line.Replace("/guilde ", "")));
                    else if (line.StartsWith("/channel"))
                        Console.WriteLine("channel=" + (channel = line.Replace("/channel ", "")));
                    else if (line.StartsWith("/send"))
                        discord.Me.Guilds[guild].Channels_list[channel].sendMessage(line.Replace("/send", "")).addReaction("♥");
                    else if (line.StartsWith("/emojis"))
                        Console.WriteLine(discord.Me.Guilds[guild].getEmojis());
                    else if (line.StartsWith("/changename"))
                        discord.Me.changeName(line.Replace("/changename ", ""));
                    else if (line.StartsWith("/delete"))
                        discord.Me.Guilds[guild].Channels_list[channel].Messages_list[line.Replace("/delete ", "")].delete();
            }
                catch (Exception e) { Console.WriteLine(e.Message); }
        }
        }
    }
}
