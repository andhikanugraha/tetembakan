using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageFormat;
using System.Collections;

namespace Client
{
    /// <summary>
    /// Interaction logic for RoomListControl.xaml
    /// </summary>
    public partial class RoomListControl : UserControl
    {
        public event EventHandler JoinRoomButtonClick;
        public event EventHandler CreateRoomButtonClick;

        public RoomListViewModel ViewModel;

        public RoomListControl()
        {
            InitializeComponent();

            ViewModel = new RoomListViewModel();

            //var room = new Room();
            //room.ID = 32;
            //room.PeerID = 32;

            //RoomViewModel.AddRoom(room);
            //RoomViewModel.AddRoom(room);
            //RoomViewModel.AddRoom(room);
            //RoomViewModel.AddRoom(room);
            RoomListView.DataContext = ViewModel;
            RoomListView.ItemsSource = ViewModel.Rooms;
        }

        public void PopulateRooms(IEnumerable<Room> rooms)
        {
            ViewModel.PopulateRooms(rooms);
        }

        public Room GetSelectedRoom()
        {
            return (Room)RoomListView.SelectedItem;
        }

        private void CreateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (CreateRoomButtonClick != null)
                CreateRoomButtonClick(this, e);
        }

        private void JoinRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (JoinRoomButtonClick != null)
            {
                JoinRoomButtonClick(this, e);

                //Console.WriteLine("Masuk join");
                //Console.WriteLine("peerID: "+ Utility.getPeerID());
                //Console.WriteLine("current room: " + GetSelectedRoom().room_id);
                Utility.getConnection().sendMessage(Message.Join(Utility.getPeerID(), GetSelectedRoom().room_id));
                byte[] message = Utility.getConnection().receive();
                Console.WriteLine("message code: " + Message.getCode(message));

                if (Message.getCode(message) == Message.SUCCESS_CODE)
                {
                    MessageBox.Show("Success to join room " + GetSelectedRoom().room_id + ".");
                }

                message = Utility.getConnection().receive();
                Console.WriteLine("message code: " + Message.getCode(message));
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // list
            //Console.WriteLine("Masuk load");
            Utility.getConnection().sendMessage(Message.List(Utility.getPeerID()));
            byte[] message = Utility.getConnection().receive();
            //Console.WriteLine("message code: " + Message.getCode(message));

            if (Message.getCode(message) == Message.ROOM_CODE)
            {
                // TODO Populate list of rooms here
                int room_count = Message.getRoomCount(message);
                Console.WriteLine("room count: " + room_count);

                ViewModel.ClearRooms();
                if (room_count != 0)
                {
                    //Console.WriteLine("message: " + message.Length);
                    ArrayList rooms = Message.getRooms(message);
                    //Console.WriteLine(rooms.Count);
                    //Console.WriteLine("roomID: " + ((Room)rooms[0]).room_id);
                    //Console.WriteLine("peerID: " + ((Room)rooms[0]).peer_id);
                    //Console.WriteLine("neighborCount: " + ((Room)rooms[0]).neighbor.Count);

                    foreach (Room room in rooms)
                    {
                        //Console.WriteLine("roomID: " + room.room_id);
                        //Console.WriteLine("peerID: " + room.peer_id);
                        //Console.WriteLine("neighborCount: " + room.neighbor.Count);
                        //foreach (Peer neighbor in room.neighbor)
                        {
                            //Console.WriteLine("neighbor: " + neighbor.peer_id);
                        }

                        ViewModel.Rooms.Add(room);
                    }
                }
            }

            message = Utility.getConnection().receive();
            Console.WriteLine("message code: " + Message.getCode(message));
        }
    }
}
