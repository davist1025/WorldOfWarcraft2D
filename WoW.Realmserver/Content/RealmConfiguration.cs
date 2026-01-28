using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;
using static WoW.Framework.Utils;

namespace WoW.Realmserver.Content
{
    public class RealmConfiguration
    {
        [JsonProperty("hostname")]
        public string IpAddress { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("world_parameters")]
        public Dictionary<string, float> WorldParameters { get; set; }

        public static RealmConfiguration Load()
        {
            if (File.Exists("./realmserver.config"))
            {
                Logger.Print("Loading Realmserver configuration...", LogEntryType.Process);
                var objData = JsonConvert.DeserializeObject<RealmConfiguration>(File.ReadAllText("./realmserver.config"));
                return objData;
            }

            Logger.Print("Failed to load realmserver.config; creating a new configuration...", LogEntryType.Process);
            var newConfig = new RealmConfiguration()
            {
                IpAddress = "127.0.0.1",
                Port = 3733,
                WorldParameters = new Dictionary<string, float>()
                {
                    { "global_movement_speed", 100f }
                }
            };
            File.WriteAllText("./realmserver.config", JsonConvert.SerializeObject(newConfig, Formatting.Indented));
            return newConfig;
        }
    }
}
