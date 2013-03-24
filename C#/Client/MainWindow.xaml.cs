using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HandshakeControl Handshake = new HandshakeControl();
        public RoomListControl RoomList = new RoomListControl();
        public CreateRoomControl CreateRoom = new CreateRoomControl();
        public InsideRoomControl InsideRoom = new InsideRoomControl();

        public MainWindow()
        {
            InitializeComponent();

            // Initialize individual controls
            InitHandshakeControl();
            InitRoomListControl();
            InitCreateRoomControl();
            InitInsideRoomControl();

            SwitchControl(Handshake);
        }

        public void SwitchControl(UserControl newControl)
        {
            Content = newControl;
        }

        public void InitHandshakeControl()
        {
            Handshake.ConnectButtonClick += new EventHandler(Handshake_Connect);
        }

        public void Handshake_Connect(object sender, EventArgs e)
        {
            HandshakeControl control = (HandshakeControl)sender;

            // TODO Connect magic here
            Debug.WriteLine(control.TrackerAddressTextBox.Text);

            RoomList.ViewModel.ClearRooms();

            // TODO Populate list of rooms here
            Random r = new Random();

            for (int i = 0; i < r.Next(10,20); ++i)
            {
                var room = new Room();
                room.ID = r.Next(1048576);
                room.PeerID = r.Next(1048576);
                RoomList.ViewModel.AddRoom(room);
            }

            // Switch to the room list
            SwitchControl(RoomList);
        }

        public void InitRoomListControl()
        {
            RoomList.CreateRoomButtonClick += new EventHandler(RoomList_CreateRoom);
            RoomList.JoinRoomButtonClick += new EventHandler(RoomList_JoinRoom);
        }

        public void RoomList_CreateRoom(object sender, EventArgs e)
        {
            Debug.WriteLine(sender);
            SwitchControl(CreateRoom);
        }

        public void RoomList_JoinRoom(object sender, EventArgs e)
        {
            // TODO magic here
            Room room = RoomList.GetSelectedRoom();

            if (room == null)
            {
                // No room was selected.
                return;
            }
            else
            {
                // A room was selected.
                // Peer ID: room.PeerID
                // Room ID: room.ID
                
                // TODO Joining magic

                // Set the contents
                // TODO replace this with databinding
                InsideRoom.RoomTitle.Text = "Room #" + room.ID.ToString();
            }

            // TODO only call the following code after success

            // TODO Change the title of the room accordingly

            SwitchControl(InsideRoom);
        }

        public void InitCreateRoomControl()
        {
            CreateRoom.Cancel += new EventHandler(CreateRoom_Cancel);
            CreateRoom.CreateRoom += new EventHandler(CreateRoom_CreateRoom);
        }

        public void CreateRoom_CreateRoom(object sender, EventArgs e)
        {
            SwitchControl(InsideRoom);
        }

        public void CreateRoom_Cancel(object sender, EventArgs e)
        {
            SwitchControl(RoomList);
        }

        public void InitInsideRoomControl()
        {
            InsideRoom.LeaveRoom += new EventHandler(InsideRoom_LeaveRoom);
        }

        public void InsideRoom_LeaveRoom(object sender, EventArgs e)
        {
            // TODO QUIT magic here


            // TODO Only call this after success
            SwitchControl(RoomList);
        }
    }
}
