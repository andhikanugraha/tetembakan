using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Client
{
    class Room
    {
        public Int32 ID { get; set; }
        public Int32 PeerID { get; set; }
        public List<Peer> Peers { get; set; }
    }

    class RoomViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Room> Rooms { get; set; }

        public RoomViewModel()
        {
            Rooms = new ObservableCollection<Room>();
        }

        public void PopulateRooms(IEnumerable<Room> rooms)
        {
            Rooms.Clear();
            foreach (var room in rooms)
            {
                Rooms.Add(room);
            }
        }
    }
}
