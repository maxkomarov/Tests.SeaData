using System.Collections.Generic;

namespace SeaData.WPF.Models
{
    public class Customer : Common.ModelBase
    {
        #region Локальные переменные
        private int id;
        private string name;
        private string inn;
        private string address;
        private List<Account> accounts = new List<Account>();
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

        public string Inn
        {
            get => inn;
            set
            {
                inn = value;
                OnPropertyChanged(() => Inn);
            }
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

        public List<Account> Accounts
        {
            get => accounts;
        }
        #endregion

        #region Ctors
        public Customer() { }

        public Customer(int id)
        {
            Id = id;
        }

        public Customer(int id, string name, string inn, string address = "")
        {
            Id = id;
            Name = name;
            Inn = inn;
            Address = address;
        }
        #endregion
    }
}
