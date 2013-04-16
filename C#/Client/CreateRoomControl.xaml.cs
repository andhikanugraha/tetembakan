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

namespace Client
{
    /// <summary>
    /// Interaction logic for CreateRoomControl.xaml
    /// </summary>
    public partial class CreateRoomControl : UserControl
    {
        public event EventHandler Cancel;
        public event EventHandler CreateRoom;

        public CreateRoomControl()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Cancel != null)
                Cancel(this, e);
        }

        private void CreateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (CreateRoom != null)
            {
                CreateRoom(this, e);
                string roomID = RoomNameTextBox.Text;
                
                Utility.getConnection().sendMessage(Message.Create(Utility.getPeerID(), Message.MAX_PLAYER_NUM, roomID));
                byte[] message = Utility.getConnection().receive();
                Console.WriteLine("message code: " + Message.getCode(message));
                if (Message.getCode(message) == Message.SUCCESS_CODE)
                {
                    Console.WriteLine("message code: " + Message.getCode(message));
                    Utility.setRoomID(roomID);
                }

                message = Utility.getConnection().receive();
                Console.WriteLine("message code: " + Message.getCode(message));
                
            }
        }
    }
}
