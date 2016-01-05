﻿using CoCSharp.Data;
using CoCSharp.Logic;
using CoCSharp.Networking;
using CoCSharp.Networking.Messages;
using System;

namespace CoCSharp.Server.Handlers
{
    public delegate void MessageHandler(CoCServer server, CoCRemoteClient client, Message message);

    public static class LoginMessageHandlers
    {
        private static void HandleLoginRequestMessage(CoCServer server, CoCRemoteClient client, Message message)
        {
            var encryptionMessage = new EncryptionMessage()
            {
                ServerRandom = CoCCrypto.CreateRandomByteArray(),
                ScramblerVersion = 1
            };

            var lrMessage = message as LoginRequestMessage;
            var lsMessage = new LoginSuccessMessage()
            {
                FacebookID = null,
                GameCenterID = null,
                MajorVersion = 7,
                MinorVersion = 200,
                RevisionVersion = 19,
                ServerEnvironment = "prod",
                LoginCount = 0,
                PlayTime = new TimeSpan(0, 0, 0),
                Unknown1 = 0,
                FacebookAppID = "297484437009394",
                DateLastPlayed = DateTime.Now,
                DateJoined = DateTime.Now,
                Unknown2 = 0,
                GooglePlusID = null,
                CountryCode = "EU"
            };

            var avatar = (Avatar)null;
            if (lrMessage.UserID == 0 && lrMessage.UserToken == null) // new account
            {
                avatar = server.AvatarManager.CreateNewAvatar();
                Console.WriteLine("Created new avatar with Token {0}, ID {1}", avatar.Token, avatar.ID);

                lsMessage.UserID = avatar.ID;
                lsMessage.UserID1 = avatar.ID;
                lsMessage.UserToken = avatar.Token;
            }
            else
            {
                if (!server.AvatarManager.Avatars.TryGetValue(lrMessage.UserToken, out avatar)) // unknown token and id
                {
                    avatar = server.AvatarManager.CreateNewAvatar(lrMessage.UserToken, lrMessage.UserID);
                    Console.WriteLine("Unknown avatar, Created new avatar with Token {0}, ID {1}", avatar.Token, avatar.ID);
                }
                else Console.WriteLine("Avatar with Token {0}, ID {1} logged in.", avatar.Token, avatar.ID);

                lsMessage.UserID = avatar.ID;
                lsMessage.UserID1 = avatar.ID;
                lsMessage.UserToken = avatar.Token;
            }

            server.AvatarManager.SaveAvatar(avatar);
            client.Avatar = avatar;

            var avatarData = new AvatarData(avatar)
            {
                TownHallLevel = 5,
                AllianceCastleLevel = 1,
                AllianceCastleTotalCapacity = 10,
                AllianceCastleUsedCapacity = 0,
            };

            var ohdMessage = new OwnHomeDataMessage()
            {
                LastVisit = TimeSpan.FromSeconds(100),
                Unknown1 = -1,
                Timestamp = DateTime.UtcNow,
                OwnAvatarData = avatarData
            };

            client.NetworkManager.SendMessage(encryptionMessage);
            client.NetworkManager.SendMessage(lsMessage); // LoginSuccessMessage
            client.NetworkManager.SendMessage(ohdMessage); // OwnHomeDataMessage
        }

        public static void RegisterLoginMessageHandlers(CoCServer server)
        {
            server.RegisterMessageHandler(new LoginRequestMessage(), HandleLoginRequestMessage);
        }
    }
}
