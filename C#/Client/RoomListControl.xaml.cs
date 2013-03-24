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
                JoinRoomButtonClick(this, e);
        }
    }
}
