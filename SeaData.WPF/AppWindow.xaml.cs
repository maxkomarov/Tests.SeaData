using System.Windows;


namespace SeaData.WPF
{
    /// <summary>
    /// Логика взаимодействия для AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        public AppWindow()
        {
            InitializeComponent();
            DataContext = ViewModels.CustomersListViewModel.GetInstance();
        }
    }
}
