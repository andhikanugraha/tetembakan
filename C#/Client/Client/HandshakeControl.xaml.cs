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
    /// Interaction logic for HandshakeControl.xaml
    /// </summary>
    public partial class HandshakeControl : UserControl
    {
        public event EventHandler ConnectButtonClick;

        public HandshakeControl()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectButtonClick != null)
                ConnectButtonClick(this, e);
        }
    }
}
