using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace DiscordBot.project_recover
{
    class Groups
    {
        private static JObject data;
        const string path = @"./storage.JSON";

        public void initStorage()
        {
            if (File.Exists(path))
                data = JObject.Parse(File.ReadAllText(path));
            else
            {
                data = new JObject();
                File.Create(path);
            }
        }

        public void saveStorage()
        {
            File.WriteAllText(path, data.ToString());
        }

        public string createGroup(string[] args, string p_user_id)
        {
            Console.WriteLine(args[0]);
            if (data.TryGetValue("Groups", out JToken groups))
                groups[args[0]] = new JObject();
            else
            {
                groups = new JObject
                {
                    [args[0]] = new JObject()
                };
            }
            data["Groups"] = groups;
            saveStorage();
            return "Done";
        }

        public string addMember(string[] args, string p_user_id)
        {
            if (args.GetLength(0) == 2)
            {
                if (data.TryGetValue("Groups", out JToken groups) && groups[args[0]] != null)
                {
                    if (groups[args[0]]["members"] == null)
                        groups[args[0]]["members"] = new JObject();
                    groups[args[0]]["members"][p_user_id] = args[1];
                    saveStorage();
                    return "Done";
                }
                else
                    return "Invalid group name";
            }
            else
                return "Usage : addMember GroupName MemberEmail";
        }

        public string createProject(string[] args, string p_user_id)
        {
            if (args.GetLength(0) == 3)
            {
                if (data.TryGetValue("Groups", out JToken groups) && groups[args[0]] != null)
                {
                    if (groups[args[0]]["projects"] == null)
                        groups[args[0]]["projects"] = new JObject();
                    groups[args[0]]["projects"][args[1]] = new JObject { ["endDate"] = args[2] };
                    saveStorage();
                    return "Done";
                }
                else
                    return "Invalid group name";
            }
            else
                return "Usage : createProject GroupName ProjectName endDate";
        }

        public string getGroups(string[] args, string p_user_id)
        {
            Console.WriteLine(args);
            if (data.TryGetValue("Groups", out JToken val))
                Console.WriteLine(val);
            else
                Console.WriteLine("No groups yey");
            return val + "\nDone";
        }

        public string join(string[] args, string p_user_id)
        {
            if (args.GetLength(0) != 1)
                return "Usage : join your.email@epitech.eu";
            if (args[0].IndexOf("@epitech.eu") > 0)
            {                
                string[] args_pass = { "vr_pool", args[0] };
                addMember(args_pass, p_user_id);
                return "Done";
            }
            else
                return "Invalide email address !";
        }

        public string checkProjects(string[] args, string p_user_id)
        {
            string returned = "\n";
            if (data.TryGetValue("Groups", out JToken groups))
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
