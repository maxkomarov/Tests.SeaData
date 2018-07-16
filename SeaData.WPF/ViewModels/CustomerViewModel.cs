using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SeaData.WPF.ViewModels
{
    public class CustomerViewModel : Common.ModelBase, IDataErrorInfo
    {
        #region Локальные переменные
        private int id;
        private string name;
        private string inn;
        private string address;
        private ObservableCollection<AccountViewModel> accounts;
        private CustomerViewModel originalValue;
        private Models.Customer customerModel;

        private ICommand updateCommand;
        private ICommand deleteCommand;
        private ICommand cancelCommand;
        #endregion

        #region Свойства
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public string Inn
        {
            get => inn;
            set
            {
                inn = value;
                OnPropertyChanged(() => Inn);
                OnPropertyChanged(() => FormattedInn);
            }
        }

        public string FormattedInn
        {
            get => decimal.Parse(inn).ToString("## ##### #####");
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(() => Address);
            }
        }

        public Mode Mode { get; set; }

        public Models.Customer CustomerModel
        {
            get => customerModel;
        }
        #endregion

        public CustomerViewModel()
        {
        }

        public CustomerViewModel(Models.Customer customer)
        {
            customerModel = customer;
            id = customer.Id;
            name = customer.Name;
            inn = customer.Inn;
            address = customer.Address;
            originalValue = (CustomerViewModel)MemberwiseClone();
        }

        public CustomersListViewModel Container
        {
            get { return CustomersListViewModel.GetInstance(); }
        }
        
        public ObservableCollection<AccountViewModel> Accounts
        {
            get { return GetAccounts(); }
            set
            {
                accounts = value;
                OnPropertyChanged(()=>Accounts);
            }
        }

        internal ObservableCollection<AccountViewModel> GetAccounts()
        {
            accounts = new ObservableCollection<AccountViewModel>();
            
            foreach (Models.Account account in (Data.SqlBridge.GetInstance().GetAccounts(customerModel)))
                accounts.Add(new AccountViewModel(account, this));
            
            return accounts;
        }


        #region Интерфейсы команд
        public ICommand UpdateCommand
        {
            get
            {
                updateCommand = updateCommand ?? new Common.CommandBase(i => this.Update(), null);
                return updateCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                deleteCommand = deleteCommand ?? new Common.CommandBase(i => this.Delete(), null);
                return deleteCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                cancelCommand = cancelCommand ?? new Common.CommandBase(i => Cancel(), null);
                return cancelCommand;
            }
        }
        #endregion

        #region Имплементация команд
        private void Update()
        {
            if (Mode == Mode.Add)
            {
                customerModel = customerModel ?? new Models.Customer();
                customerModel.Name = Name;
                customerModel.Inn = Inn;
                customerModel.Address = Address;
                
                Data.SqlBridge.GetInstance().Update(customerModel);
                Accounts = GetAccounts();
            }
            else if (Mode == Mode.Edit)
            {
                customerModel.Name = Name;
                customerModel.Inn = Inn;
                customerModel.Address = Address;

                Data.SqlBridge.GetInstance().Update(customerModel);

                originalValue = (CustomerViewModel)MemberwiseClone();
            }
        }

        private void Delete()
        {
            //Удаляем запись о счете из БД
            Data.SqlBridge.GetInstance().Delete(customerModel);
            //Обновляем список счетов клиента
            Container.CustomerList = CustomersListViewModel.GetInstance().GetCustomers();
        }

        private void Cancel()
        {
            //В режиме редактирования возвращаем оригинальные значения 
            if (Mode == Mode.Edit)
            {
                Name = originalValue.Name;
                Inn = originalValue.Inn;
                Address = originalValue.Address;
            }
        }
        #endregion


        #region имплементация интерфейса IDataErrorInfo
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Name")
                {
                    if (!Regex.IsMatch(Name, @"[\sа-яА-Я1-9]+$"))
                        return "Допустимы только символы кириллицы";
                    if (Name.Length > 100)
                        return "Максимальная длина - 100 символов";
                    if (Name.Length == 0)
                        return "Поле не может быть пустым";
                }
                else if (columnName == "Inn")
                {
                    long inn;
                    if (!long.TryParse(Inn, out inn))
                        return "ИНН должен выражаться числом";
                    else
                    {
                        if (inn < 100000000000 | inn > 999999999999)
                            return "ИНН должен состоять из 12 цифр";
                    }
                }
                else if (columnName == "Address")
                {
                    if (Address.Length > 255)
                        return "Максимальная длина - 255 символов";
                }
                return null;
            }
        }
        #endregion
    }
}
