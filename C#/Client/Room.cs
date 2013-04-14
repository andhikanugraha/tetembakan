using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using MessageFormat;

namespace Client
{
    /*
    public class Room
    {
        public Int32 ID { get; set; }
        public Int32 PeerID { get; set; }
        // public List<Peer> Peers { get; set; }
    }
    */

    public class RoomListViewModel
    {
        public ObservableCollection<Room> Rooms { get; set; }
        public Room CurrentRoom { get; set; }

        public RoomListViewModel()
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

        public void ClearRooms()
        {
            Rooms.Clear();
        }

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }

        public void PropertyChanged()
        {
        }
    }

    public class InsideRoomViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Room _room;

        public Room Room
        {
            get
            {
                return this._room;
            }

            set
            {
                if (value != this._room)
                {
                    this._room = value;
                    RoomChanged();
                }
            }
        }

        private void RoomChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Room"));
            }
        }

        public InsideRoomViewModel(Room room = null)
        {
            Room = room;
        }
    }
}
