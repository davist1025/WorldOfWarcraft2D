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
using WoW.Network.Objects;
using WoW.Network.Packets;
using WoW.Network.Packets.Client;
using static WoW.Framework.Utils;

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

        public List<ChatMessageObject> ChatHistory = new List<ChatMessageObject>();
        public List<ChatMessageObject> GMChatHistory = new List<ChatMessageObject>();
        public List<RealmserverMetadataObject> Realmlist = new List<RealmserverMetadataObject>();
        public List<CharacterMetadataObject> Characters = new List<CharacterMetadataObject>();

        public override void OnAddedToEntity()
        {
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(DrawGUI);

            _chatChannels = Enum.GetNames<ChatChannelType>();
        }

        private void DrawGUI()
        {
            Scene currentScene = Entity.Scene;

            switch (Global.OnlineState)
            {
                case GameNetworkState.Offline:
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(300f, 125f));
                    ImGui.Begin("Login");
                    ImGui.InputText("Account Name", ref _accountNameInput, 32);
                    ImGui.InputText("Password", ref _accountPasswordInput, 64, ImGuiInputTextFlags.Password);

                    if (ImGui.Button("Connect"))
                    {
                        Global.Network.ConnectTo(port: 8070);
                        Global.OnlineState = GameNetworkState.Auth_LoggingIn;
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
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(300f, 125f));
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
                            RealmserverMetadataObject realmserver = Realmlist[i];

                            if (ImGui.Selectable($"##{realmserver.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    Global.OnlineState = GameNetworkState.Realm;
                                    Game1.LastConnectedRealm = realmserver;

                                    // todo: save last used realm for auto-connection later.
                                    //Game1.Network.Disconnect();
                                    Global.Network.ConnectTo(realmserver.Hostname, realmserver.Port);
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
                            CharacterMetadataObject character = Characters[i];

                            if (ImGui.Selectable($"##{character.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                _characterSelectIndex = i;
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    Global.OnlineState = GameNetworkState.LoadingWorld;
                                    Global.Network.SendToServer(new ClientRealm_TransferWorld() { LocalCharacterId = character.Id });
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text($"{character.Name}");
                            ImGui.NextColumn();
                            ImGui.Text($"{character.Race}");
                            ImGui.NextColumn();
                            ImGui.Text($"{character.Hair}");
                            ImGui.NextColumn();
                        }
                        ImGui.Columns(0);
                    }

                    ImGui.SetCursorPosX((ImGui.GetWindowSize().X / 2f) / 2f);
                    if (ImGui.Button("Create Character"))
                        Global.OnlineState = GameNetworkState.Realm_CreateCharacter;

                    if (_characterSelectIndex > -1)
                    {
                        if (ImGui.Button("Delete Character"))
                        {
                            Global.Network.SendToServer(new ClientRealm_DeleteCharacter() { CharacterId = _characterSelectIndex });
                            Global.OnlineState = GameNetworkState.Realm;
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

                    var raceTypeNames = Enum.GetNames<ActorRaceType>();
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
                        Global.Network.SendToServer(new ClientRealm_CreateCharacter() 
                        { 
                            Name = _newCharacterNameInput.Trim(),
                            RaceId = _newCharacterRaceId + 1,
                            HairId = _newCharacterHairId
                        });
                        Global.OnlineState = GameNetworkState.Realm;
                    }

                    if (NezImGui.CenteredButton("Back", 0.5f))
                    {
                        Global.Network.SendToServer(new ClientRealm_RequestCharacterList());
                        Global.OnlineState = GameNetworkState.Realm;
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
                        Global.Network.SendToServer(new ClientRealm_RequestCharacterList());
                        Global.OnlineState = GameNetworkState.Realm;
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
                            ImGui.Text($"{Global.SessionId}");
                            ImGui.Text($"Map Id: {Game1.ActiveMapId}");
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
                            var color = WoW.Framework.Utils.ChannelColors[chatHistory.Channel];

                            ImGui.PushStyleColor(ImGuiCol.Text, color);
                            ImGui.TextWrapped(chatHistory.Input);
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
                            Global.Network.SendToServer(new ChatMessageObject
                            {
                                Input = sanitizedInput,
                                Channel = (ChatChannelType)_chatChannelIndex
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
                                    ImGui.Text(chatHistory.Input);
                                    //ImGui.PopStyleColor();
                                }
                                ImGui.SetScrollHereY(1f);

                                ImGui.EndChild();
                            }

                            ImGui.End();
                        }

                        if (Game1.ShowShowBackpack)
                        {
                            var backpackWindowSize = new System.Numerics.Vector2(50f, 150f);
                            ImGui.SetNextWindowSize(backpackWindowSize);
                            ImGui.Begin("Backpack", ImGuiWindowFlags.NoResize);



                            ImGui.End();
                        }
                    }

                    break;
            };
        }

        public void Update()
        {
            if (Input.IsKeyPressed(Global.Config.KeyboardControlMap[Content.ControlMap.EscapeMenu]))
                Global.Config.ControlHandlers[Content.ControlMap.EscapeMenu]?.Invoke(null, null);
        }

        public string GetLogin()
            => $"{_accountNameInput.Trim()}:{_accountPasswordInput.Trim()}";
    }
}
