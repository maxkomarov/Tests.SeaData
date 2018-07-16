using System.Windows;

namespace SeaData.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для AccountView.xaml
    /// </summary>
    public partial class AccountView : Window
    {
        public AccountView()
        {
            InitializeComponent();
        }

        private void bUpdate_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
