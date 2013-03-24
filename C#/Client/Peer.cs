using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Client
{
    class Peer
    {
        public string Name { get; set; }
        public string ID { get; set; }

        Peer(string Name, string ID)
        {
            this.Name = Name;
            this.ID = ID;
        }
    }
}
