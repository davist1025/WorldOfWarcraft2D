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
        private int _chatChannelIndex = 0;
        private string[] _chatChannels;

        private string _newCharacterNameInput = "";
        private int _newCharacterRaceId = 1;
        private int _newCharacterHairId = 1;

        private int _characterSelectIndex = -1;

        private string[] _onlineCharacters = new[] { "" };

        /// <summary>
        /// Does not automatically update.
        /// 
        /// Usually populated upon a /who command.
        /// </summary>
        public string[] OnlineCharacters
        {
            get => _onlineCharacters;
            set
            {
                Game1.ShouldShowWhoMenu = true;
                _onlineCharacters = value;
            }
        }

        public List<ChatMessage> ChatHistory = new List<ChatMessage>();
        public List<ChatMessage> GMChatHistory = new List<ChatMessage>();
        public List<RemoteRealmserver> Realmlist = new List<RemoteRealmserver>();
        public List<RemoteCharacter> Characters = new List<RemoteCharacter>();

        public override void OnAddedToEntity()
        {
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(DrawGUI);

            _chatChannels = Enum.GetNames<ChatChannel>();
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
                    ImGui.Text("Retrieving characters...");
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

                    if (NezImGui.CenteredButton("Create", 0.5f))
                    {
                        Game1.Send(new ClientRealm_CreateCharacter() 
                        { 
                            Name = _newCharacterNameInput.Trim(),
                            RaceId = _newCharacterRaceId + 1,
                            HairId = _newCharacterHairId
                        });
                        Game1.NetState = GameNetworkState.Realm;
                    }

                    if (NezImGui.CenteredButton("Back", 0.5f))
                    {
                        Game1.Send(new ClientRealm_RequestCharacterList());
                        Game1.NetState = GameNetworkState.Realm;
                    }

                    ImGui.End();
                    break;
                case GameNetworkState.Realm_CharacterNameInvalid:
                    var dialogSize2 = new System.Numerics.Vector2(450, 50);

                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(Core.GraphicsDevice.Viewport.Width / 2 - (dialogSize2.X / 2), Core.GraphicsDevice.Viewport.Height / 2 - (dialogSize2.Y / 2)));
                    ImGui.SetNextWindowSize(dialogSize2);

                    ImGui.Begin("dialog", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);

                    ImGui.Text($"This character name is invalid. Please try again.");

                    if (ImGui.Button("Ok"))
                    {
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
                            ImGui.Text($"{controller.Name}");
                            ImGui.Text($"{Game1.SessionId}");
                            ImGui.Text($"Map Id: {Game1.CurrentMapId}");

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
                        for (int i = 0; i < ChatHistory.Count; i++)
                        {
                            var chatHistory = ChatHistory[i];
                            var color = Shared.Utils.ChatChannelColors[chatHistory.Channel];

                            ImGui.PushStyleColor(ImGuiCol.Text, color);
                            ImGui.TextWrapped(chatHistory.Message);
                            ImGui.PopStyleColor();
                        }
                        ImGui.SetScrollHereY(1f);

                        ImGui.EndChild();
                    }

                    ImGui.Separator();

                    ImGui.SetNextItemWidth(75f);
                    if (ImGui.BeginCombo("##combo", _chatChannels[_chatChannelIndex]))
                    {
                        // todo: remove "Server," "Support" from gui.
                        for (int i = 0; i < _chatChannels.Length; i++)
                        {
                            if (ImGui.Selectable(_chatChannels[i]))
                            {
                                _chatChannelIndex = i;
                            }
                        }
                        ImGui.EndCombo();
                    }

                    ImGui.SameLine();

                    // process chat input upon pressing enter.
                    if (ImGui.InputTextWithHint("", "Type message here...", ref _chatInput, 128, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        var sanitizedInput = _chatInput.Trim();
                        _chatInput = "";

                        if (!string.IsNullOrEmpty(sanitizedInput))
                        {
                            // todo: determine a function for using different channels.
                            Game1.Send(new ChatMessage
                            {
                                Message = sanitizedInput,
                                Channel = (ChatChannel)_chatChannelIndex
                            });

                            sanitizedInput = "";
                        }
                    }

                    ImGui.End();

                    // todo: finish escape menu.
                    if (Game1.ShouldShowEscapeMenu)
                    {
                        System.Numerics.Vector2 windowSize = new System.Numerics.Vector2(200, 200);
                        System.Numerics.Vector2 position = new System.Numerics.Vector2(Core.GraphicsDevice.Viewport.Width / 2 - (windowSize.X / 2), Core.GraphicsDevice.Viewport.Height / 2 - (windowSize.Y / 2));

                        ImGui.SetNextWindowSize(windowSize);
                        ImGui.SetNextWindowPos(position);
                        ImGui.Begin("", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);

                        // todo: display game client options.

                        // show player/npc names, etc.
                        NezImGui.CenteredButton("Options", 0.75f);

                        // todo: key bindings.
                        NezImGui.CenteredButton("Key Bindings", 0.75f);

                        // instantly closes the game.
                        // server will auto-detect disconnection.
                        if (NezImGui.CenteredButton("Logout", 0.75f))
                            Game1.Exit();

                        ImGui.End();
                    }

                    if (!Game1.ShouldShowEscapeMenu)
                    {
                        if (Game1.ShouldShowWhoMenu)
                        {
                            var whoWinSize = new System.Numerics.Vector2(275f, 400f);

                            ImGui.SetNextWindowSize(whoWinSize);
                            ImGui.Begin("Online Players", ref Game1.ShouldShowWhoMenu);

                            // todo: add columns for level, race, location (map name) in a future revision.
                            for (int i = 0; i < OnlineCharacters.Length; i++)
                                ImGui.Text(OnlineCharacters[i]);

                            // todo: buttons for refresh, close?

                            ImGui.End();
                        }

                        if (Game1.ShouldShowGMChat)
                        {
                            var gmChatSize = new System.Numerics.Vector2(325f, 115f);

                            ImGui.SetNextWindowSize(gmChatSize);
                            ImGui.Begin("Talking with a GM");

                            if (ImGui.BeginChild("chat_output", new System.Numerics.Vector2(0f, -30), true))
                            {
                                for (int i = 0; i < GMChatHistory.Count; i++)
                                {
                                    var chatHistory = GMChatHistory[i];
                                    //var color = Shared.Utils.ChatChannelColors[chatHistory.Channel];

                                    //ImGui.PushStyleColor(ImGuiCol.Text, color);
                                    ImGui.Text(chatHistory.Message);
                                    //ImGui.PopStyleColor();
                                }
                                ImGui.SetScrollHereY(1f);

                                ImGui.EndChild();
                            }

                            ImGui.End();
                        }
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
