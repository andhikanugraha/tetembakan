using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public static class Utility
    {
        private static Connection connection;
        private static int peerID;

        public static Connection getConnection()
        {
            if (connection == null)
            {
                connection = new Connection("127.0.0.1");
            }

            return connection;
        }

        public static int getPeerID()
        {
            return peerID;
        }

        public static void setConnection(string IPAddress)
        {
            connection = new Connection(IPAddress);
        }

        public static void setPeerID(int new_peerID)
        {
            peerID = new_peerID;
        }
    }
}
