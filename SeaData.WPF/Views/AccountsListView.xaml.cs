using SeaData.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SeaData.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для AccountsListView.xaml
    /// </summary>
    public partial class AccountsListView : UserControl
    {
        public AccountsListView()
        {
            InitializeComponent();
        }

        private void bAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountView view = new AccountView();
            AccountViewModel account = new AccountViewModel();
            account.CustomerViewModel = (CustomerViewModel)DataContext;
            account.Mode = Mode.Add;
            view.DataContext = account;
            view.ShowDialog();
        }

        private void bEditAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountView view = new AccountView();
            AccountViewModel account = (AccountViewModel)((MenuItem)sender).DataContext;
            account.Mode = Mode.Edit;
            view.DataContext = account;
            view.ShowDialog();
        }
    }
}
