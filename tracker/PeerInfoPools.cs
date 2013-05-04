using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GunbondGame
{
    class peerInfoPools
    {
        private bool[] pools;
        private const int maxPools = 5000;
        private bool[] ports;

        public peerInfoPools()
        {
            pools = new bool[maxPools];
            ports = new bool[maxPools];
        }

        public int getAvailableID(){
            for (int i = 0; i < maxPools; i++)
            {
                if (!pools[i])
                {
                    pools[i] = true;
                    return i+1;
                }
            }
            return -1;
        }

        public void releaseID(int peerID, int port)
        {
            pools[peerID - 1] = false;
            ports[port - 13000] = false;
        }

        public int getAvailablePort()
        {
            for (int i = 0; i < ports.Length; i++)
            {
                if (!ports[i])
                {
                    ports[i] = true;
                    return i + 13000;
                }
            }
            return -1;
        }
    }
}
