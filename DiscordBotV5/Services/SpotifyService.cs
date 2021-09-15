using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotV5.Services
{
    public class SpotifyService
    {
        private readonly IConfiguration _config;

        public SpotifyClient _spotifyClient;

        public SpotifyService(IServiceProvider service)
        {
            _config = service.GetRequiredService<IConfiguration>();
        }

        public void Initialize()
        {
            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(_config["SpotifyClientID"], _config["SpotifyClientSecret"]));

                _spotifyClient = new SpotifyClient(config);

        }

    }
}
