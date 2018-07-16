using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SeaData.WPF.ViewModels 
{
    public class AccountViewModel : Common.ModelBase, IDataErrorInfo
    {
        #region Локальные переменные
        private int id;
        private string number;
        private string name;
        private string bic;
        private string saldo;
        private CustomerViewModel customerViewModel;
        private AccountViewModel originalValue;
        private Models.Account accountModel;

        private ICommand updateCommand;
        private ICommand deleteCommand;
        private ICommand cancelCommand;
        #endregion

        #region Свойства
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged(() => Id);
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public string Number
        {
            get => number;
            set
            {
                number = value;
                OnPropertyChanged(() => Number);
                OnPropertyChanged(() => FormattedNumber);
            }
        }

        public string BIC
        {
            get => bic; 
            set
            {
                bic = value.Replace(" ","");
                OnPropertyChanged(() => BIC);
                OnPropertyChanged(() => FormattedBIC);
            }
        }

        public string Saldo
        {
            get => saldo;
            set
            {
                saldo = value;
                OnPropertyChanged(() => Saldo);
                OnPropertyChanged(() => FormattedSaldo);
            }
        }

        public string FormattedNumber { get => decimal.Parse(number).ToString("##### ##### ##### #####"); }
        public string FormattedBIC { get => long.Parse(bic).ToString("## ### ####"); }
        public string FormattedSaldo { get => decimal.Parse(saldo).ToString("N", CultureInfo.CurrentCulture); }

        public CustomerViewModel CustomerViewModel
        {
            get => customerViewModel;
            set
            {
                customerViewModel = value;
                OnPropertyChanged(() => CustomerViewModel);
            }
        }

        #endregion

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
                cancelCommand = cancelCommand ?? new Common.CommandBase(i => this.Undo(), null);
                return cancelCommand;
            }
        }
        #endregion

        #region Имплементация команд
        private void Update()
        {
            if (Mode == Mode.Add)
            {
                accountModel = accountModel ?? new Models.Account();
                accountModel.Number = Number;
                accountModel.BIC = BIC;
                accountModel.Saldo = decimal.Parse(Saldo);
                accountModel.Name = Name;
                accountModel.Owner = customerViewModel.CustomerModel;

                Data.SqlBridge.GetInstance().Update(accountModel);
                customerViewModel.Accounts = customerViewModel.GetAccounts();
            }
            else if (Mode == Mode.Edit)
            {
                accountModel.Number = Number;
                accountModel.BIC = BIC;
                accountModel.Saldo = decimal.Parse(Saldo);
                accountModel.Name = Name;
                accountModel.Owner = customerViewModel.CustomerModel;

                Data.SqlBridge.GetInstance().Update(accountModel);
                
                originalValue = (AccountViewModel)MemberwiseClone();
            }
        }

        private void Delete()
        {
            //Удаляем запись о счете из БД
            Data.SqlBridge.GetInstance().Delete(accountModel);
            //Обновляем список счетов клиента
            customerViewModel.Accounts = customerViewModel.GetAccounts();
        }

        private void Undo()
        {
            //В режиме редактирования возвращаем оригинальные значения 
            if (Mode == Mode.Edit)
            {
                Number = originalValue.Number;
                BIC = originalValue.BIC;
                Saldo = originalValue.Saldo;
                Name = originalValue.Name;
            }
        }
        #endregion

        public AccountViewModel()
        {

        }

        public AccountViewModel(Models.Account accountModel, CustomerViewModel customerViewModel)
        {
            this.accountModel = accountModel;
            Id = accountModel.Id;
            Number = accountModel.Number;
            BIC = accountModel.BIC;
            Name = accountModel.Name;
            Saldo = accountModel.Saldo.ToString();
            CustomerViewModel = customerViewModel;
            originalValue = (AccountViewModel)MemberwiseClone();
        }

        public Mode Mode
        {
            get;
            set;
        }

        #region имплементация интерфейса IDataErrorInfo
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Number")
                {
                    double number;
                    if (!double.TryParse(Number, out number))
                        return "Номер счета должен выражаться числом";
                    else
                    {
                        if (number < 10000000000000000000 | number > 99999999999999999999f)
                            return "Номер счета должен состоять из 20 цифр";
                    }
                }
                else if (columnName == "BIC")
                {
                    int bic;
                    if (!int.TryParse(BIC, out bic))  
                        return "БИК должен выражаться числом";
                    else
                    {
                        if (bic < 100000000 | bic > 999999999)
                            return "БИК должен состоять из 9 цифр";
                    }
                }
                else if (columnName == "Saldo")
                {
                    decimal saldo;
                    if (!decimal.TryParse(Saldo, out saldo)) 
                        return "Остаток должен выражаться десятичным числом";
                    else
                    {
                        if (saldo > 1000000000000 | saldo < 0)
                            return "Остаток должен быть < 1 000 000 000 000";
                    }
                }
                else if (columnName == "Name")
                {
                    if (!Regex.IsMatch(Name, @"[\sа-яА-Я1-9]+$"))
                        return "Допустимы только символы кириллицы";
                    if (Name.Length > 100)
                        return "Максимальная длина поля - 100 символов";
                }
                return null;
            }
        }
        #endregion
    }
}
