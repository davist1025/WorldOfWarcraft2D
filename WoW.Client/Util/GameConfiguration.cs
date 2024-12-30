using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Nez;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Util
{
    public class GameConfiguration
    {
        [JsonProperty("keyboard_map")]
        public Dictionary<ControlMap, Keys> KeyboardControlMap { get; set; }

        [JsonIgnore]
        public Dictionary<ControlMap, EventHandler> ControlHandlers;

        // todo: account name, last realm info.

        public void Save()
        {
            Debug.Log("Writing configuration to disk...");

            var jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText("./game.config", jsonData);
        }

        public static GameConfiguration Load()
        {
            if (File.Exists("./game.config"))
            {
                Debug.Log("Loading game configuration...");
                var objData = JsonConvert.DeserializeObject<GameConfiguration>(File.ReadAllText("./game.config"));
                return objData;
            }

            Debug.Log("Failed to find game.config; creating a new one...");
            return new GameConfiguration()
            {
                KeyboardControlMap = new Dictionary<ControlMap, Keys>()
                {
                    { ControlMap.TabTarget, Keys.Tab }
                }
            };
        }
    }

    public enum ControlMap
    {
        TabTarget
    }
}
