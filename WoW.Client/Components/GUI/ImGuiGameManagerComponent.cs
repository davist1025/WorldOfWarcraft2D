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
using WoW.Client.Network;
using WoW.Client.Utils;
using WoW.Framework.Network.Container;
using static WoW.Framework.Utils;

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

        private GameManager _gameManager;

        public override void OnAddedToEntity()
        {
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(Draw);
            _gameManager = Core.GetGlobalManager<GameManager>();
        }

        public void Update()
        {
            
        }

        private void Draw()
        {
            switch (_gameManager.PeerState)
            {
                case GameNetworkState.World:
                    var thePlayerController = Entity.Scene.FindComponentOfType<MyPlayerControllerComponent>();

                    ImGui.SetNextWindowPos(new Vector2(10f).ToNumerics());
                    ImGui.SetNextWindowBgAlpha(0.5f);
                    if (ImGui.Begin("#information", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.Text("World of Warcraft 2D - Debug");
                        ImGui.Separator();
                        ImGui.Text($"{_gameManager.NetworkId}");
                        ImGui.Text($"Ping: {_gameManager.Network.GetPing()}");

                        ImGui.End();
                        // todo: [gui] recreate the information window.
                        //ImGui.Text($"{thePlayerController.Name}");
                        //ImGui.Text($"{Global.SessionId}");
                        //ImGui.Text($"Map Id: {Game1.ActiveMapId}");
                        //ImGui.End();
                    }

                    var chatSize = new System.Numerics.Vector2(425f, 190f);

                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(10f, Core.GraphicsDevice.Viewport.Height - chatSize.Y - 10f));
                    ImGui.SetNextWindowSize(chatSize);
                    ImGui.SetNextWindowBgAlpha(0.5f);

                    ImGui.Begin("Chat");

                    if (ImGui.BeginChild("chat_output", new System.Numerics.Vector2(0f, -30), true))
                    {
                        for (int i = 0; i < ChatHistory.Count; i++)
                        {
                            var chatHistory = ChatHistory[i];
                            //var color = WoW.Framework.Utils.ChannelColors[chatHistory.Channel];

                            //ImGui.PushStyleColor(ImGuiCol.Text, color);
                            ImGui.TextWrapped(chatHistory.Input);
                            //ImGui.PopStyleColor();
                        }
                        ImGui.SetScrollHereY(1f);

                        ImGui.EndChild();
                    }

                    ImGui.Separator();

                    // process chat input upon pressing enter.
                    if (ImGui.InputTextWithHint("", "Type message here...", ref _chatInput, 128, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        var sanitizedInput = _chatInput.Trim();
                        _chatInput = "";

                        if (!string.IsNullOrEmpty(sanitizedInput))
                        {
                            NetPacketManager.BuildChatMessage(sanitizedInput);
                            //// todo: determine a function for using different channels.
                            //Global.Network.SendToServer(new ChatMessageObject
                            //{
                            //    Input = sanitizedInput,
                            //    Channel = (ChatChannelType)_chatChannelIndex
                            //});

                            sanitizedInput = "";
                        }
                    }

                    ImGui.End();
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
