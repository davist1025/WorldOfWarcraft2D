using ImGuiNET;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
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
using WoW.Client.Shared.Data;

namespace WoW.Client.Components
{
    public class ImGuiController : Component, IUpdatable
    {
        private string _chatInput = "";
        private string _accountNameInput = "";

        private string _newCharacterNameInput = "";

        private int _characterSelectIndex = -1;

        public List<string> Chat = new List<string>();
        public List<RemoteRealmserver> Realmlist = new List<RemoteRealmserver>();
        public List<RemoteCharacter> Characters = new List<RemoteCharacter>();

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
                            RemoteRealmserver realmserver = Realmlist[i];

                            if (ImGui.Selectable($"##{realmserver.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    Game1.NetState = GameNetworkState.Realm;
                                    Game1.LastRealm = realmserver;

                                    // todo: save last used realm for auto-connection later.
                                    Game1.ClientNetwork.DisconnectAll();
                                    Game1.ClientNetwork.Connect(realmserver.Hostname, realmserver.Port, "");
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text(realmserver.Name);
                            ImGui.NextColumn();
                            ImGui.Text(realmserver.Hostname);
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
                    ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(250f, 150f), new System.Numerics.Vector2(400f, 250f));
                    ImGui.Begin("Characters");

                    if (Characters.Count > 0)
                    {
                        ImGui.Columns(1);
                        ImGui.Text("Name");
                        ImGui.NextColumn();

                        for (int i = 0; i < Characters.Count; i++)
                        {
                            RemoteCharacter character = Characters[i];

                            if (ImGui.Selectable($"##{character.CharacterName}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                _characterSelectIndex = character.CharacterId;
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    Game1.NetState = GameNetworkState.LoadingWorld;
                                    Game1.Send(new ClientRealm_TransferWorld() { LocalCharacterId = character.CharacterId });
                                    //var netScene = new NetworkTestScene();
                                    //var netScene = new NetworkTestScene(character);

                                    //Core.StartSceneTransition(new FadeTransition(() => netScene));

                                    //Game1.NetState = GameNetworkState.World;
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text($"{character.CharacterName}");
                            ImGui.NextColumn();
                        }
                        ImGui.Columns(0);
                    }

                    ImGui.SetCursorPosX((ImGui.GetWindowSize().X / 2f) / 2f);
                    if (ImGui.Button("Create Character"))
                        Game1.NetState = GameNetworkState.Realm_CreateCharacter;

                    if (_characterSelectIndex > -1)
                    {
                        if (ImGui.Button("Delete Character"))
                        {
                            Game1.Send(new ClientRealm_DeleteCharacter() { CharacterId = _characterSelectIndex });
                            Game1.NetState = GameNetworkState.Realm;
                            _characterSelectIndex = -1;
                        }
                    }

                    // todo: finish disconnect button.
                    // set the network state to offline, disconnect.
                    ImGui.SetCursorPosX((ImGui.GetWindowSize().X / 2f) / 2f);
                    if (ImGui.Button("Disconnect"))
                        Game1.Disconnect();

                    ImGui.End();
                    break;
                case GameNetworkState.Realm_CreateCharacter:
                    ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(175f, 100f), new System.Numerics.Vector2(250f, 175f));
                    ImGui.Begin("Create Character");

                    ImGui.InputText("Name", ref _newCharacterNameInput, 12);

                    if (NezImGui.CenteredButton("Create", 0.5f))
                    {
                        Game1.Send(new ClientRealm_CreateCharacter() { Name = _newCharacterNameInput.Trim() });
                        Game1.NetState = GameNetworkState.Realm;
                    }

                    if (NezImGui.CenteredButton("Back", 0.5f))
                    {
                        // todo: the client will need to ask for the character list, again. i dont think a packet exists for that :p

                    }

                    ImGui.End();
                    break;
                case GameNetworkState.LoadingWorld:
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, Game1.GraphicsDevice.Viewport.Height - 40));
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(185, 25));
                    ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);
                    ImGui.Text("Receiving data...");
                    ImGui.End();
                    break;
                case GameNetworkState.World:
                    ImGuiWindowFlags infoWindowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize;

                    ImGui.SetNextWindowBgAlpha(0.5f);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(x: 10f, y: 10f));

                    var localPlayer = Entity.Scene.FindEntity("thePlayer");

                    if (localPlayer != null)
                    {
                        var controller = localPlayer.GetComponent<LocalPlayerController>();

                        if (ImGui.Begin("information", infoWindowFlags))
                        {
                            ImGui.Text($"Position: {controller.Entity.Transform.Position.ToString()}");
                            ImGui.Text($"Server Position: {controller.LastServerPosition}");

                            ImGui.End();
                        }
                    }
                    break;
                    //case GameNetworkState.World:
                    //    ImGui.SetNextWindowSize(new System.Numerics.Vector2(425, 190));

                    //    ImGui.Begin("Chat");

                    //    if (ImGui.BeginChild("chat_output", new System.Numerics.Vector2(0f, -30), true))
                    //    {
                    //        for (int i = 0; i < Chat.Count; i++)
                    //            ImGui.TextUnformatted(Chat[i]);

                    //        ImGui.EndChild();
                    //    }

                    //    ImGui.Separator();

                    //    if (ImGui.InputText("Input", ref _chatInput, 125, ImGuiInputTextFlags.EnterReturnsTrue) && !string.IsNullOrWhiteSpace(_chatInput))
                    //    {
                    //        // todo: print our own chat.
                    //        // should we just have the server send us back our own message?
                    //        Game1.Send(new ClientRealm_Chat() { Message = _chatInput }, LiteNetLib.DeliveryMethod.ReliableOrdered);
                    //        _chatInput = "";
                    //    }

                    //    ImGui.End();
                    //    break;
            };
        }

        public void Update()
        {

        }
    }
}
