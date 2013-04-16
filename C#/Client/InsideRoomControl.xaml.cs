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
            ParticipantsListView.DataContext = ViewModel;
            ParticipantsListView.ItemsSource = ViewModel.Room.neighbor;

            RoomTitle.Text = "Room #" + ViewModel.Room.room_id;
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Loaded.");
            /*
            Utility.getConnection().sendMessage(Message.RefreshInsideRoom(Utility.getPeerID()));
            byte[] message = Utility.getConnection().receive();
            Console.WriteLine("message code: " + Message.getCode(message));
            if (Message.getCode(message) == Message.INSIDEROOM_CODE)
            {
                Room room = Message.getRoom(message);
                MessageBox.Show("Success refreshing room " + room.room_id);
                ViewModel.Room = room;
            }
            else 
            {
                MessageBox.Show("message code: " + Message.getCode(message));
            }

            message = Utility.getConnection().receive();
            Console.WriteLine("message code: " + Message.getCode(message));
            */
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Utility.getConnection().sendMessage(Message.RefreshInsideRoom(Utility.getPeerID()));
            byte[] message = Utility.getConnection().receive();
            Console.WriteLine("message code: " + Message.getCode(message));
            if (Message.getCode(message) == Message.INSIDEROOM_CODE)
            {
                Room room = Message.getRoom(message);
                MessageBox.Show("Success refreshing room " + room.neighbor.Count);
                ViewModel.Room = room;
            }

            message = Utility.getConnection().receive();
            Console.WriteLine("message code: " + Message.getCode(message));
        }
    }
}
