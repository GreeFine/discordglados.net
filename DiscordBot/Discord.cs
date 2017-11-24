using DiscordBot;
using System.Collections.Generic;

namespace AREA.API
{
    public class Discord
    {
        private const string redirect_uri = "https://localhost:44329/Deck/SetUpSocialNetwork?api=Discord";
        public const string auth_url_ = "https://discordapp.com/oauth2/authorize?client_id=380310980808409089&scope=bot%20identify&response_type=code&permissions=137612352&redirect_uri=" + redirect_uri;

        public Me Me = Me.instance;
        public Users Users = Users.instance;
        public Gateway Gateway = Gateway.instance;
    }
}