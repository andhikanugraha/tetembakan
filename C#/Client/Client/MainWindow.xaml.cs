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

        public MainWindow()
        {
            InitializeComponent();

            // Initialize individual controls
            InitHandshakeControl();
            InitRoomListControl();
            InitCreateRoomControl();

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

            // TODO Populate list of rooms here

            // TODO Only call this after connected
            SwitchControl(RoomList);
        }

        public void InitRoomListControl()
        {
            RoomList.CreateRoomButtonClick += new EventHandler(RoomList_CreateRoom);
        }

        public void RoomList_CreateRoom(object sender, EventArgs e)
        {
            Debug.WriteLine(sender);
            SwitchControl(CreateRoom);
        }

        public void InitCreateRoomControl()
        {
            CreateRoom.CancelButtonClick += new EventHandler(CreateRoom_Cancel);
        }

        public void CreateRoom_Cancel(object sender, EventArgs e)
        {
            SwitchControl(RoomList);
        }
    }
}
