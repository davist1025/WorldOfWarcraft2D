using ImGuiNET;
using Nez;
using Nez.BitmapFonts;
using Nez.ImGuiTools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components.Player;
using WoW.Client.Network;
using WoW.Client.Scenes;
using WoW.Client.Utils;
using WoW.Framework.Logging;
using WoW.Framework.Network.Container;
using static WoW.Framework.Utils;

namespace WoW.Client.Components.GUI
{
    /// <summary>
    /// Handles the imgui implementation for the various states of the main menu.
    /// </summary>
    internal class ImGuiMainMenuManagerComponent : Component
    {
        private string _accountNameInput = "";
        private string _accountPasswordInput = "";

        private string _newCharacterNameInput = "";
        private int _newCharacterRaceId = 1;
        private int _newCharacterHairId = 1;

        private int _characterSelectIndex = -1;

        private GameManager _gameManager;

        public override void OnAddedToEntity()
        {
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(Draw);

            _gameManager = Core.GetGlobalManager<GameManager>();
        }

        public void Draw()
        {
            var windowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize;

            switch (_gameManager.PeerState)
            {
                case GameNetworkState.Offline:
                    var windowSize = new System.Numerics.Vector2(300f, 175f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f, 
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));
                    ImGui.Begin("login", windowFlags);
                    ImGui.InputText("Account Name", ref _accountNameInput, 32);
                    ImGui.InputText("Password", ref _accountPasswordInput, 64, ImGuiInputTextFlags.Password);

                    NezImGui.SmallVerticalSpace();

                    if (NezImGui.CenteredButton("Login", 0.6f))
                        _gameManager.LogonClicked?.Invoke(null, (_accountNameInput, _accountPasswordInput));


                    if (NezImGui.CenteredButton("Offline-mode", 0.6f))
                    {
                        // todo: move offline-mode button press functionality to GameManager.
                        //Logger.Print($"Running the game in offline-mode.", LogEntryType.Process);

                        //Global.PeerState = GameNetworkState.Offline_World;
                        //Core.StartSceneTransition(new FadeTransition(() => new WorldScene()));
                    }

                    if (NezImGui.CenteredButton("Quit", 0.6f)) Core.Exit();

                    ImGui.End();
                    break;
                case GameNetworkState.Auth_LoggingIn:
                    windowSize = new System.Numerics.Vector2(125f, 25f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f, 
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));
                    ImGui.Begin("debug_text", windowFlags);
                    ImGui.Text("Logging in...");
                    ImGui.End();
                    break;
                case GameNetworkState.Auth_Realmlist:
                    windowSize = new System.Numerics.Vector2(400f, 135f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f,
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));

                    if (_gameManager.Realmlist.Count > 0)
                    {
                        ImGui.Begin("Realmlist", windowFlags);
                        ImGui.Columns(3);
                        ImGui.Text("Name");
                        ImGui.NextColumn();
                        ImGui.Text("IP");
                        ImGui.NextColumn();
                        ImGui.Text("Port");
                        ImGui.NextColumn();

                        for (int i = 0; i < _gameManager.Realmlist.Count; i++)
                        {
                            RealmserverContainer realmserver = _gameManager.Realmlist[i];

                            if (ImGui.Selectable($"##{realmserver.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    // todo: move realmserver click functionality to GameManager.
                                    _gameManager.PeerState = GameNetworkState.Realm;
                                    _gameManager.Network.ConnectTo(new string[] { realmserver.Hostname, realmserver.Port.ToString() });
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
                    windowSize = new System.Numerics.Vector2(185f, 25f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f, 
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));
                    ImGui.Begin("debug_text", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);
                    ImGui.Text("Retrieving characters...");
                    ImGui.End();
                    break;
                case GameNetworkState.Realm_Characters:
                    windowSize = new System.Numerics.Vector2(250f, 175f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f,
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));

                    ImGui.Begin("characters", windowFlags);

                    MyOnlineComponent localOnlineComponent = _gameManager.Player.GetComponent<MyOnlineComponent>();

                    if (localOnlineComponent.Characters.Count > 0)
                    {
                        ImGui.Columns(3);
                        ImGui.Text("Name");
                        ImGui.NextColumn();
                        ImGui.Text("Race");
                        ImGui.NextColumn();
                        ImGui.Text("Hair");
                        ImGui.NextColumn();

                        for (int i = 0; i < localOnlineComponent.Characters.Count; i++)
                        {
                            CharacterContainer character = localOnlineComponent.Characters[i];

                            if (ImGui.Selectable($"##{character.Name}", false, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                localOnlineComponent.SetSelectedCharacter(i);
                                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                                {
                                    // todo: move character click functionality to GameManager.
                                    _gameManager.PeerState = GameNetworkState.LoadingWorld;
                                    NetPacketManager.BuildEnterWorld(character);
                                }
                            }

                            ImGui.SameLine();
                            ImGui.Text($"{character.Name}");
                            ImGui.NextColumn();
                            ImGui.Text($"{character.RaceId}");
                            ImGui.NextColumn();
                            ImGui.Text($"{character.HairId}");
                            ImGui.NextColumn();
                        }
                        ImGui.Columns(1);
                    }

                    NezImGui.SmallVerticalSpace();

                    if (NezImGui.CenteredButton("Create Character", 0.6f))
                        _gameManager.PeerState = GameNetworkState.Realm_CreateCharacter;

                    if (_characterSelectIndex > -1)
                    {
                        if (NezImGui.CenteredButton("Delete Character", 0.6f))
                        {
                            // todo: send character deletion request; move delete character functionality to GameManager.
                            //Global.Network.SendToServer(new ClientRealm_DeleteCharacter() { CharacterId = _characterSelectIndex });
                            _gameManager.PeerState = GameNetworkState.Realm;
                            _characterSelectIndex = -1;
                        }
                    }

                    if (NezImGui.CenteredButton("Disconnect", 0.6f))
                        _gameManager.Disconnected?.Invoke(null, null);

                    ImGui.End();
                    break;
                case GameNetworkState.Realm_CreateCharacter:
                    windowSize = new System.Numerics.Vector2(200f, 150f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f,
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));

                    ImGui.Begin("create_character", windowFlags);

                    ImGui.InputText("Name", ref _newCharacterNameInput, 16);

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

                    NezImGui.SmallVerticalSpace();

                    if (NezImGui.CenteredButton("Create", 0.6f))
                    {
                        // todo: send character creation request.
                        //Global.Network.SendToServer(new ClientRealm_CreateCharacter()
                        //{
                        //    Name = _newCharacterNameInput.Trim(),
                        //    RaceId = _newCharacterRaceId + 1,
                        //    HairId = _newCharacterHairId
                        //});
                        _gameManager.PeerState = GameNetworkState.Realm;
                    }

                    if (NezImGui.CenteredButton("Back", 0.6f))
                    {
                        // todo: send character list request.
                        //Global.Network.SendToServer(new ClientRealm_RequestCharacterList());
                        _gameManager.PeerState = GameNetworkState.Realm;
                    }

                    ImGui.End();
                    break;
                case GameNetworkState.LoadingWorld:
                    windowSize = new System.Numerics.Vector2(185f, 25f);

                    ImGui.SetNextWindowSize(windowSize);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(
                        Core.GraphicsDevice.Viewport.Width / 2f - windowSize.X / 2f, 
                        Core.GraphicsDevice.Viewport.Height / 2f - windowSize.Y / 2f));
                    ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoCollapse);
                    ImGui.Text("Entering world...");
                    ImGui.End();
                    break;

                case GameNetworkState.Offline_Realm:
                    break;
            }
        }

        public string GetLogin()
            => $"{_accountNameInput.Trim()}:{_accountPasswordInput.Trim()}";
    }
}
