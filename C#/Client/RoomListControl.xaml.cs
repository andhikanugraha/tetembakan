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

        public RoomListControl()
        {
            InitializeComponent();
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
