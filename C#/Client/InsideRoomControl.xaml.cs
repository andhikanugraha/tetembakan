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
            RoomTitle.Text = "Room #" + ViewModel.Room.ID.ToString();
        }

        private void LeaveRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (LeaveRoom != null)
                LeaveRoom(this, e);
        }
    }
}
