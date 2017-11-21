using AREA.API;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Users
    {
        public static readonly Users instance = new Users();
        private Users() { }

        Core Core = Core.instance;

        public JArray getUser(string p_id)
        {
            return Core.Get("/users/" + p_id);
        }
    }
}
