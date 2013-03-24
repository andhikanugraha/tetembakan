using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Client
{
    class Peer
    {
        public string Name { public get; private set; }
        public string ID { public get; private set; }

        Peer(string Name, string ID)
        {
            this.Name = Name;
            this.ID = ID;
        }
    }
}
