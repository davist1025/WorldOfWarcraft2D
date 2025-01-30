using ImGuiNET;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using System;
using System.Collections.Generic;
using System.IO;
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
        private string _accountPasswordInput = "";

        private string _newCharacterNameInput = "";
        private int _newCharacterRaceId = 1;
        private int _newCharacterHairId = 1;

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
                    ImGui.InputText("Password", ref _accountPasswordInput, 64, ImGuiInputTextFlags.Password);
                    // todo: password.
                    //ImGui.InputText("Passowrd")

                    if (ImGui.Button("Connect"))
                    {
                        Game1.ConnectAndLogin(_accountNameInput, _accountPasswordInput);
                        //Game1.AccountName = _accountNameInput;
                        //Game1.ClientNetwork.Connect("127.0.0.1", 8070, "");
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
                case GameNetworkState.Auth_Invalid:
                    var dialogSize = new System.Numerics.Vector2(450, 50);

                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(Core.GraphicsDevice.Viewport.Width / 2 - (dialogSize.X / 2), Core.GraphicsDevice.Viewport.Height / 2 - (dialogSize.Y / 2)));
                    ImGui.SetNextWindowSize(dialogSize);

                    ImGui.Begin("dialog", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);

                    ImGui.Text($"Your account name or password are incorrect. Please try again.");

                    if (ImGui.Button("Ok"))
                        Game1.Disconnect();

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
                        ImGui.Columns(3);
                        ImGui.Text("Name");
                        ImGui.NextColumn();
                        ImGui.Text("Race");
                        ImGui.NextColumn();
                        ImGui.Text("Hair");
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
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text($"{character.CharacterName}");
                            ImGui.NextColumn();
                            ImGui.Text($"{character.RaceId}");
                            ImGui.NextColumn();
                            ImGui.Text($"{character.HairId}");
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

                    ImGui.SetCursorPosX((ImGui.GetWindowSize().X / 2f) / 2f);
                    if (ImGui.Button("Disconnect"))
                        Game1.Disconnect();

                    ImGui.End();
                    break;
                case GameNetworkState.Realm_CreateCharacter:
                    ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(175f, 100f), new System.Numerics.Vector2(250f, 175f));
                    ImGui.Begin("Create Character");

                    ImGui.InputText("Name", ref _newCharacterNameInput, 12);

                    var raceTypeNames = Enum.GetNames<RaceType>();
                    ImGui.Combo("Race", ref _newCharacterRaceId, raceTypeNames, raceTypeNames.Length);

                    var hairFiles = Directory.GetFiles("Content/Data/Characters/").Where(f => f.ToLower().Contains("hair")).ToArray();

                    if (ImGui.Button("<-"))
                    {
                        if (_newCharacterHairId == 1)
                            _newCharacterHairId = hairFiles.Length;
                        else
                            _newCharacterHairId--;
                    }
                    ImGui.SameLine();
                    ImGui.Text($"Hair: {_newCharacterHairId}");
                    ImGui.SameLine();
                    if (ImGui.Button("->"))
                    {
                        if (_newCharacterHairId == hairFiles.Length)
                            _newCharacterHairId = 1;
                        else
                            _newCharacterHairId++;
                    }

                    Debug.Log(_newCharacterRaceId);

                    if (NezImGui.CenteredButton("Create", 0.5f))
                    {
                        Game1.Send(new ClientRealm_CreateCharacter() 
                        { 
                            Name = _newCharacterNameInput.Trim(),
                            RaceId = _newCharacterRaceId,
                            HairId = _newCharacterHairId
                        });
                        Game1.NetState = GameNetworkState.Realm;
                    }

                    if (NezImGui.CenteredButton("Back", 0.5f))
                    {
                        // todo: the client will need to ask for the character list, again. i dont think a packet exists for that :p
                        Game1.Send(new ClientRealm_RequestCharacterList());
                        Game1.NetState = GameNetworkState.Realm;
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

                    var localPlayer = Entity.Scene.FindComponentOfType<LocalPlayerController>();

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

                    var chatSize = new System.Numerics.Vector2(425f, 190f);

                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(10f, Core.GraphicsDevice.Viewport.Height - chatSize.Y - 10f));
                    ImGui.SetNextWindowSize(chatSize);
                    ImGui.SetNextWindowBgAlpha(0.5f);

                    ImGui.Begin("Chat");

                    if (ImGui.BeginChild("chat_output", new System.Numerics.Vector2(0f, -30), true))
                    {
                        for (int i = 0; i < Chat.Count; i++)
                            ImGui.TextUnformatted(Chat[i]);
                        ImGui.SetScrollHereY(1f);

                        ImGui.EndChild();
                    }

                    ImGui.Separator();

                    if (ImGui.InputText("Input", ref _chatInput, 125, ImGuiInputTextFlags.EnterReturnsTrue) && !string.IsNullOrWhiteSpace(_chatInput))
                    {
                        if (_chatInput.StartsWith("/"))
                        {
                            var rawInput = _chatInput.Substring(1);
                            var splitInput = rawInput.Split(' ');

                            if (splitInput[0].ToLower().Equals("whisper"))
                            {
                                string message = "";
                                for (int i = 2; i < splitInput.Length; i++)
                                {
                                    if (i == splitInput.Length - 1)
                                        message += $"{splitInput[i]}";
                                    else
                                        message += $"{splitInput[i]} ";
                                }

                                ClientRealm_Chat newChatWhisper = new ClientRealm_Chat()
                                {
                                    IsWhisper = true,
                                    Message = message,
                                    Name = splitInput[1]
                                };
                                Game1.Send(newChatWhisper);
                            }
                        }
                        else
                        {
                            Game1.Send(new ClientRealm_Chat() { Message = _chatInput });
                        }
                        _chatInput = "";
                    }

                    ImGui.End();

                    if (Game1.ShouldShowEscapeMenu)
                    {
                        System.Numerics.Vector2 windowSize = new System.Numerics.Vector2();
                        System.Numerics.Vector2 position = new System.Numerics.Vector2();

                        Debug.Log("Drawing escape!");
                    }

                    break;
            };
        }

        public void Update()
        {
            if (Input.IsKeyPressed(Game1.Configuration.KeyboardControlMap[Content.ControlMap.EscapeMenu]))
                Game1.Configuration.ControlHandlers[Content.ControlMap.EscapeMenu]?.Invoke(null, null);
        }
    }
}
