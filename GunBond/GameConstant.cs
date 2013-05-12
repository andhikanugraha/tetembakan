using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunBond
{
    class GameConstant
    {
        /* MESSAGE */
        public const string pstr = "GunbondGame";
        public const int handshakeCode = 135;
        public const int handshakeResponseCode = 136;
        public const int keepAliveCode = 182;
        public const int createCode = 255;
        public const int listCode = 254;
        public const int roomCode = 200;
        public const int successCode = 127;
        public const int failedCode = 128;
        public const int joinCode = 253;
        public const int startCode = 252;
        public const int quitCode = 235;
        public const int beginRoomCode = 14;
        public const int endRoomCode = 12;
        public const int checkCode = 177;
        public const int checkResponseCode = 178;
        public const int joinPeerCode = 133;
        public const int handshakeCreatorCode = 111;
        public const int playerCode = 112;
        public const int joinSuccessCode = 250;
        public const int creatorQuitCode = 222;
        public const int RoomInfoCode = 123;
        public const int SelfPromoteCreatorCode = 117;
        public const int GameUpdate = 118;

        public static byte[] pstrByte = Encoding.ASCII.GetBytes(pstr);
        public static byte[] reservedByte = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

		public const int roomIDSize = 50;
		public const int startSize = 54;
        public const int IPAddressSize = 4;
        public const int peerIdSize = 4;
        public const int maxPlayerSize = 4;
        public const int timestampSize = 8;
        public const int roomCountSize = 4;
        public const int handshakeMsgSize = 20;
        public const int checkMsgSize = 20;
        public const int portSize = 4;
        public const int usernameSize = 10;
        public const int peerInfoSize = IPAddressSize + peerIdSize + portSize + usernameSize;
        public const int gameUpdateSize = 36;

        /* NETWORK */
        public const int defaultMaxPeer = 100;
        public const int defaultMaxRoom = 100;
        public const int trackerPort = 12345;
        public const int maxConnectionQueue = 10;
        public const int defaultPort = 13000;
        public const int keepAliveInterval = 3000;
        public const int connectionTimeOut = 10000;
    }
}
