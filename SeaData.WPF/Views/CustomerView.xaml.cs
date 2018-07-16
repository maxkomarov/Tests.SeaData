using System.Windows;

namespace SeaData.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для CustomerView.xaml
    /// </summary>
    public partial class CustomerView : Window
    {
        public CustomerView()
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
