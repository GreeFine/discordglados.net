using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace DiscordBot.project_recover
{
    class Groups
    {
        const string path = @"./storage.JSON";
        private static Core.Storage storage = new Core.Storage(path);

        private static JObject data;
        public JObject Data { get { if (data == null) data = storage.get(); return data; } set { data = value; } }

        public string createGroup(string[] args, Message message)
        {
            if (Data.TryGetValue("Groups", out JToken groups))
                groups[args[0]] = new JObject();
            else
            {
                groups = new JObject
                {
                    [args[0]] = new JObject()
                };
            }
            Data["Groups"] = groups;
            return "Done";
        }

        public string addMember(string[] args, Message message)
        {
            if (args.GetLength(0) == 2)
            {
                if (Data.TryGetValue("Groups", out JToken groups) && groups[args[0]] != null)
                {
                    if (groups[args[0]]["members"] == null)
                        groups[args[0]]["members"] = new JObject();
                    groups[args[0]]["members"][message.author_id] = args[1];
                    storage.save(Data);
                    return "Done";
                }
                else
                    return "Invalid group name";
            }
            else
                return "Usage : addMember GroupName MemberEmail";
        }

        public string createProject(string[] args, Message message)
        {
            if (args.GetLength(0) == 3)
            {
                if (Data.TryGetValue("Groups", out JToken groups) && groups[args[0]] != null)
                {
                    if (groups[args[0]]["projects"] == null)
                        groups[args[0]]["projects"] = new JObject();
                    groups[args[0]]["projects"][args[1]] = new JObject { ["endDate"] = args[2] };
                    storage.save(Data);
                    return "Done";
                }
                else
                    return "Invalid group name";
            }
            else
                return "Usage : createProject GroupName ProjectName endDate";
        }

        public string getGroups(string[] args, Message message)
        {
            Data.TryGetValue("Groups", out JToken val);
            return val + "\nDone";
        }

        public string join(string[] args, Message message)
        {
            if (args.GetLength(0) != 1)
                return "Usage : join your.email@epitech.eu";
            if (args[0].IndexOf("@epitech.eu") > 0)
            {
                string[] args_pass = { "vr_pool", args[0] };
                addMember(args_pass, message);
                return "Done";
            }
            else
                return "Invalide email address !";
        }

        public string checkProjects(string[] args, Message message)
        {
            string returned = "\n";
            if (Data.TryGetValue("Groups", out JToken groups))
            {
                foreach (JProperty member in groups["vr_pool"]["members"])
                {
                    string email = member.First.Value<string>();
                    string member_uid = member.Name;
                    const string project_name = "vr_pool";
                    RepoManager.cloneEpitech(email, project_name);
                    if (Directory.Exists(email + "/" + project_name))
                        returned += "<@" + member_uid + "> Validated :white_check_mark:\n";
                    else
                        returned += "<@" + member_uid + "> Invalid :x:\n";
                }
            }
            return returned;
        }
    }
}
