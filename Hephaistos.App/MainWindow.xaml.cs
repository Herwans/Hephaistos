using Hephaistos.App.Mvvm.ViewModel;
using System.Windows;

namespace Hephaistos.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel vm = new();
            DataContext = vm;
        }
    }
}