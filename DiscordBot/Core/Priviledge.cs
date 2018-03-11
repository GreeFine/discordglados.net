using Newtonsoft.Json.Linq;
using System.IO;

namespace DiscordBot
{
    class Priviledge
    {
        const string path = @"./Priviledge.JSON";
        private static Core.Storage storage = new Core.Storage(path);

        private static JObject data;
        private static JObject Data { get { if (data == null) data = storage.get(); return data; } set { data = value; } }


        public static bool isAdmin(string p_id)
        {
            return (Data["Admin"] != null && Data["Admin"][p_id] != null);
        }

        public static string getAdmins()
        {
            string list = "\n```";
            if (Data["Admin"] == null)
                return "Empty";
            foreach (var admin in Data["Admin"])
            {
                list += admin.ToString() + "\n";
            }
            return list + "```";
        }

        public static string addAdmin(string[] args)
        {
            if (args.Length != 2)
                return "Invalid";
            if (Data["Admin"] == null)
                Data["Admin"] = new JObject();
            Data["Admin"][args[0]] = args[1];
            storage.save(Data);
            return "Done";
        }
        
    }
}
