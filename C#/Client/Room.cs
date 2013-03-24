using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Client
{
    class Room
    {
        public Peer Creator { get; set; }
        public string Title { get; set; }
        public List<Peer> Peers { get; set; }
    }
}
