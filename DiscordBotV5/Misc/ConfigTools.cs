using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBotV5.Misc
{
    class ConfigTools
    {
        public IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        public void generateNewConfig()
        {
            ConfigTemplate template = new ConfigTemplate();
            string json = JsonConvert.SerializeObject(template, Formatting.Indented);
            File.WriteAllText("config.json", json);
        }
    }
}
