using Newtonsoft.Json.Linq;
using System.IO;

namespace DiscordBot
{
    class Priviledge
    {
        private static JObject data;
        private static JObject Data { get { if (data == null) initStorage(); return data; } set { data = value; } }
        const string path = @"./Priviledge.JSON";

        public static void initStorage()
        {
            if (File.Exists(path))
                data = JObject.Parse(File.ReadAllText(path));
            else
            {
                data = new JObject();
                File.Create(path);
            }
        }

        public static void saveStorage()
        {
            File.WriteAllText(path, Data.ToString());
        }

        public static bool isAdmin(string p_id)
        {
            return (Data["Admin"] != null && Data["Admin"][p_id] != null);
        }

        public static string getAdmins()
        {
            string list = "\n```";
            foreach (var admin in Data["Admin"])
            {
                list += admin.ToString() + "\n";
            }
            return list + "```";
        }
    }
}
