using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Client
{
    class Peer2
    {
        public string Name { get; set; }
        public string ID { get; set; }

        Peer2(string Name, string ID)
        {
            this.Name = Name;
            this.ID = ID;
        }
    }
}
