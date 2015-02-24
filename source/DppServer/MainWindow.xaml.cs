using DistPoServer.Core;
using System.Windows;

namespace DistPoServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var model = new ServerModel();
            var viewModel = new ServerViewModel(model, Dispatcher);
            DataContext = viewModel;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
