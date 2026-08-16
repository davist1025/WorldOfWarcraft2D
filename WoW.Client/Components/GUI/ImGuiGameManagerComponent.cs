using ImGuiNET;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Nez;
using Nez.ImGuiTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components.Player;
using WoW.Framework.Network.Container;
using static WoW.Client.Global;

namespace WoW.Client.Components.GUI
{
    /// <summary>
    /// Handles ImGui during gameplay.
    /// </summary>
    internal class ImGuiGameManagerComponent : Component, IUpdatable
    {
        private string _chatInput = "";
        private int _chatChannelIndex = 0;
        private string[] _chatChannels;
        public List<ChatMessageContainer> ChatHistory = new List<ChatMessageContainer>();

        public override void OnAddedToEntity()
        {
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(Draw);
        }

        public void Update()
        {
            
        }

        private void Draw()
        {
            switch (Global.PeerState)
            {
                case GameNetworkState.World:
                    var thePlayerController = Entity.Scene.FindComponentOfType<MyPlayerControllerComponent>();

                    ImGui.SetNextWindowPos(new Vector2(10f).ToNumerics());
                    ImGui.SetNextWindowBgAlpha(0.5f);
                    if (ImGui.Begin("#information", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.Text("World of Warcraft 2D - Debug");
                        ImGui.Separator();
                        ImGui.Text($"{Global.NetworkId}");
                        ImGui.Text($"Ping: {Global.Network.GetPing()}");

                        ImGui.End();
                        // todo: [gui] recreate the information window.
                        //ImGui.Text($"{thePlayerController.Name}");
                        //ImGui.Text($"{Global.SessionId}");
                        //ImGui.Text($"Map Id: {Game1.ActiveMapId}");
                        //ImGui.End();
                    }
                    break;
                case GameNetworkState.Offline:
                    ImGui.Begin("#offline_info", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize);

                    ImGui.Text("World of Warcraft 2D - Debug (Offline)");
                    ImGui.Separator();

                    ImGui.End();
                    break;
            }
        }
    }
}
