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

        public MainWindow()
        {
            InitializeComponent();

            // Initialize individual controls
            InitHandshakeControl();
            InitRoomListControl();

            SwitchControl(Handshake);
        }

        public void SwitchControl(UserControl newControl)
        {
            Content = newControl;
        }

        public void InitHandshakeControl()
        {
            Handshake.OnConnectButtonClick += new HandshakeControl.ConnectHandler(HandleConnect);
        }

        public void HandleConnect(HandshakeControl control)
        {
            Debug.WriteLine(control.TrackerAddressTextBox.Text);
            SwitchControl(RoomList);
        }

        public void InitRoomListControl()
        {

        }
    }
}
