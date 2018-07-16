using SeaData.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SeaData.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для CustomersListView.xaml
    /// </summary>
    public partial class CustomersListView : UserControl
    {
        public CustomersListView()
        {
            InitializeComponent();
        }

        private void bAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerView view = new CustomerView();
            CustomerViewModel customer = new CustomerViewModel();
            customer.Mode = Mode.Add;
            view.DataContext = customer;  
            view.ShowDialog();
        }

        private void bEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerView view = new CustomerView();
            CustomerViewModel customer = (CustomerViewModel)((MenuItem)sender).DataContext;
            customer.Mode = Mode.Edit;
            view.DataContext = customer;
            view.ShowDialog();
        }
    }
}
