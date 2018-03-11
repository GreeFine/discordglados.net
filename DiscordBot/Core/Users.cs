using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class Users
    {
        public static readonly Users instance = new Users();
        private Users() { }

        public JArray getUser(string p_id)
        {
            return DiscordWebRequest.Get("/users/" + p_id);
        }
    }
}
