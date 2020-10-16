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

        public void GenerateNewConfig()
        {
            ConfigTemplate template = new ConfigTemplate();
            string json = JsonConvert.SerializeObject(template, Formatting.Indented);
            File.WriteAllText("config.json", json);
        }

        public bool configExists()
        {
            if (!File.Exists("config.json"))
            {
                return false;
            }
            return true;
        }

        public IConfiguration Build()
        {
            if (configExists())
            {
                return BuildConfig();
            }
            else
            {
                GenerateNewConfig();
                Environment.Exit(0); // no point in continuing because the bot will error out without a token
                return null; // have to return something to get rid of compiler error even tho it will never reach it
            }
        }
    }
}
