using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for InsideRoomControl.xaml
    /// </summary>
    public partial class InsideRoomControl : UserControl
    {
        public event EventHandler LeaveRoom;

        public InsideRoomViewModel ViewModel;

        public InsideRoomControl()
        {
            InitializeComponent();

            ViewModel = new InsideRoomViewModel();
            ViewModel.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChange);
        }

        public void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            RoomTitle.Text = "Room #" + ViewModel.Room.room_id.ToString();
        }

        private void LeaveRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (LeaveRoom != null)
            {
                LeaveRoom(this, e);

                //Console.WriteLine("Masuk join");
                //Console.WriteLine("peerID: "+ Utility.getPeerID());
                //Console.WriteLine("current room: " + GetSelectedRoom().room_id);
                Utility.getConnection().sendMessage(Message.Quit(Utility.getPeerID()));
                byte[] message = Utility.getConnection().receive();
                Console.WriteLine("message code: " + Message.getCode(message));

                if (Message.getCode(message) == Message.SUCCESS_CODE)
                {
                    MessageBox.Show("Success to leave the room.");
                }

                message = Utility.getConnection().receive();
                Console.WriteLine("message code: " + Message.getCode(message));
            }
        }
    }
}
