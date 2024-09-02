using ImGuiNET;
using LiteNetLib.Utils;
using Nez;
using Nez.ImGuiTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Scenes;
using WoW.Client.Shared;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Components
{
    public class ImGuiController : Component, IUpdatable
    {
        private string _chatInput = "";
        private string _accountNameInput = "";

        public List<string> Chat = new List<string>();
        public List<Realmserver> Realmlist = new List<Realmserver>();
        public List<SerializableCharacter> Characters = new List<SerializableCharacter>();

        public override void OnAddedToEntity()
        {
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(DrawGUI);
        }

        private void DrawGUI()
        {
            Scene currentScene = Entity.Scene;

            switch (Game1.NetState)
            {
                case GameNetworkState.Offline:
                    ImGui.Begin("Login");
                    ImGui.InputText("Account Name", ref _accountNameInput, 32);
                    // todo: password.
                    //ImGui.InputText("Passowrd")

                    if (ImGui.Button("Connect"))
                    {
                        Game1.AccountName = _accountNameInput;
                        Game1.ClientNetwork.Connect("127.0.0.1", 8070, "");
                    }
                    ImGui.End();
                    break;
                case GameNetworkState.Auth_LoggingIn:
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, Game1.GraphicsDevice.Viewport.Height - 40));
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(125, 25));
                    ImGui.Begin("debug_text", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);
                    ImGui.Text("Logging in...");
                    ImGui.End();
                    break;
                case GameNetworkState.Auth_Realmlist:
                    if (Realmlist.Count > 0)
                    {

                        ImGui.Begin("Realmlist");
                        ImGui.Columns(3);
                        ImGui.Text("Name");
                        ImGui.NextColumn();
                        ImGui.Text("IP");
                        ImGui.NextColumn();
                        ImGui.Text("Port");
                        ImGui.NextColumn();

                        for (int i = 0; i < Realmlist.Count; i++)
                        {
                            Realmserver realmserver = Realmlist[i];

                            if (ImGui.Selectable($"##{realmserver.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    Game1.NetState = GameNetworkState.Realm;
                                    Game1.LastRealm = realmserver;

                                    Game1.ClientNetwork.DisconnectAll();
                                    Game1.ClientNetwork.Connect(realmserver.Ip, realmserver.Port, "");
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text(realmserver.Name);
                            ImGui.NextColumn();
                            ImGui.Text(realmserver.Ip);
                            ImGui.NextColumn();
                            ImGui.Text(realmserver.Port.ToString());
                        }
                        ImGui.End();
                    }
                    break;
                case GameNetworkState.Realm:
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, Game1.GraphicsDevice.Viewport.Height - 40));
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(185, 25));
                    ImGui.Begin("debug_text", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);
                    ImGui.Text("Retrieving characters..."); // todo: fix this not being long enough.
                    ImGui.End();
                    break;
                case GameNetworkState.Realm_Characters:
                    ImGui.Begin("Characters");
                    if (Characters.Count > 0)
                    {
                        ImGui.Columns(3);
                        ImGui.Text("Name");
                        ImGui.NextColumn();
                        ImGui.Text("Level");
                        ImGui.NextColumn();
                        ImGui.Text("Class");
                        ImGui.NextColumn();

                        for (int i = 0; i < Characters.Count; i++)
                        {
                            SerializableCharacter character = Characters[i];

                            if (ImGui.Selectable($"##{character.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    var netScene = new NetworkTestScene(character);

                                    /** todo: world transfer.
                                     * 
                                     * tell the server what character we want to use, server will send gear, friends list, etc
                                     * we will join the world as that character.
                                     * 
                                     */

                                    Core.StartSceneTransition(new FadeTransition(() => netScene));

                                    Game1.NetState = GameNetworkState.World;
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text($"{character.Name}");
                            ImGui.NextColumn();
                            ImGui.Text(character.Level.ToString());
                            ImGui.NextColumn();
                            ImGui.Text(character.Class.ToString());
                        }
                        ImGui.Columns(0);
                    }
                    ImGui.Button("Create Character");

                    ImGui.End();
                    break;
                case GameNetworkState.World:
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(425, 190));

                    ImGui.Begin("Chat");

                    if (ImGui.BeginChild("chat_output", new System.Numerics.Vector2(0f, -30), true))
                    {
                        for (int i = 0; i < Chat.Count; i++)
                            ImGui.TextUnformatted(Chat[i]);

                        ImGui.EndChild();
                    }

                    ImGui.Separator();

                    if (ImGui.InputText("Input", ref _chatInput, 125, ImGuiInputTextFlags.EnterReturnsTrue) && !string.IsNullOrWhiteSpace(_chatInput))
                    {
                        // todo: print our own chat.
                        // should we just have the server send us back our own message?
                        Game1.Send(new ClientRealm_Chat() { Message = _chatInput }, LiteNetLib.DeliveryMethod.ReliableOrdered);
                        _chatInput = "";
                    }

                    ImGui.End();
                    break;
            };
        }

        public void Update()
        {

        }
    }
}
