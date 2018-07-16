using SeaData.WPF.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SeaData.WPF.ViewModels
{
    public class CustomersListViewModel : Common.ModelBase
    {
        private static CustomersListViewModel instance = null;
        private static Data.SqlBridge bridge = null;

        private CustomerViewModel selectedCustomer = null;
        private ObservableCollection<CustomerViewModel> customerList = null;

        private ICommand showAddCommand;
        private ICommand showEditCommand;

        /// <summary>
        /// Внутрення коллекция экземпляров, для представления в строках грида
        /// </summary>
        public ObservableCollection<CustomerViewModel> CustomerList
        {
            get => GetCustomers();
            set
            {
                customerList = value;
                OnPropertyChanged(()=>CustomerList);
            }
        }

        /// <summary>
        /// Текущий выбор в гриде - для привязки данных грида со счетами
        /// </summary>
        public CustomerViewModel SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                OnPropertyChanged(()=> SelectedCustomer);
            }
        }

        public ICommand ShowAddCommand
        {
            get
            {
                showAddCommand = showAddCommand ?? new Common.CommandBase(i => ShowDialog(), null);
                return showAddCommand;
            }
        }
        public ICommand ShowEditCommand
        {
            get
            {
                showEditCommand = showEditCommand ?? new Common.CommandBase(i => ShowDialog(selectedCustomer), null);
                return showEditCommand;
            }
        }

        /// <summary>
        /// Ctor. Инициализирует шлюз к данным и заполняет коллекцию
        /// </summary>
        private CustomersListViewModel()
        {
            bridge = Data.SqlBridge.GetInstance();
            CustomerList = GetCustomers();
        }

        /// <summary>
        /// Singleton, однако... :)
        /// </summary>
        /// <returns></returns>
        public static CustomersListViewModel GetInstance()
        {
            return instance ?? new CustomersListViewModel();
        }

        /// <summary>
        /// Заполняет коллекцию данными
        /// </summary>
        /// <returns></returns>
        internal ObservableCollection<CustomerViewModel> GetCustomers()
        {
            customerList = customerList ?? new ObservableCollection<CustomerViewModel>();
            customerList.Clear();
            foreach (Models.Customer customer in bridge.GetCustomers())
                customerList.Add(new CustomerViewModel(customer));
            return customerList;
        }

        private void ShowDialog(CustomerViewModel customer = null)
        {
            CustomerView view = new CustomerView();

            if (customer == null)
            {
                customer = new CustomerViewModel();
                customer.Mode = Mode.Add;
            }
            else
                customer.Mode = Mode.Edit;

            view.DataContext = customer;
            view.ShowDialog();
            if (view.DataContext != null)
                GetCustomers();

        }
    }
}
