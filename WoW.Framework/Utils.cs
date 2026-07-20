using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Framework
{
    /// <summary>
    /// Potentially globally accessed functions.
    /// </summary>
    public static class Utils
    {
        #region Actor definitions
        public enum ActorRaceType
        {
            Human = 1,
            Orc,
        }

        public enum ActorClassType
        {
            Warrior = 1,
            Mage,
        }

        public enum ActorType
        {
            Local,
            Networked,
            Mob
        }

        public enum ActorFlagTypes
        {
            None = -1
        }

        public enum ActorAnimationDirection
        {
            North = 1,
            East,
            South,
            West
        }
        #endregion

        #region Network definitions

        public enum ChatChannelType
        {
            Say,
            Yell,
            Whisper,
            World,
            Server
        }

        public enum AccountSecurityType
        {
            Player = 1,
            Gamemaster,
            Administrator
        }

        public enum AuthCodeType : int
        {
            Success,
            NoRecord,
            InvalidPassword,
            AlreadyOnline
        }

        #endregion

        #region Logger definitions

        public enum LogEntryType
        {
            Debug,
            Error,
            Warning,
            Fatal,
            Network,
            Process
        }

        #endregion

        public static string ToSha256(string input)
        {
            string shaHash = "";

            using (var hash = SHA256.Create())
            {
                var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                shaHash = Convert.ToHexString(byteArray).ToLower();
            }

            return shaHash;
        }

        public static Dictionary<ChatChannelType, System.Numerics.Vector4> ChannelColors = new Dictionary<ChatChannelType, System.Numerics.Vector4>()
        {
            { ChatChannelType.Say, new System.Numerics.Vector4(255 / 255f, 255f / 255f, 255f / 255f, 1f) },
            { ChatChannelType.Server, new System.Numerics.Vector4(250f / 255f, 244f / 255f, 125f / 255f, 1f) }
        };
    }
}
