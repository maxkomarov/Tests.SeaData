namespace SeaData.WPF.Models
{
    public class Account : Common.ModelBase
    {
        #region Локальные переменные
        private int id;
        private string number;
        private string name;
        private string bic;
        private decimal saldo;
        private Customer owner;
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
            }
        }

        public string BIC
        {
            get => bic;
            set
            {
                bic = value;
                OnPropertyChanged(() => BIC);
            }
        }

        public decimal Saldo
        {
            get => saldo;
            set
            {
                saldo = value;
                OnPropertyChanged(() => Saldo);
            }
        }

        public Customer Owner
        {
            get => owner;
            set
            {
                owner = value;
                OnPropertyChanged(() => Owner);
            }
        }

        #endregion

        #region Ctors
        public Account() { }

        public Account(Customer owner, int id, string number, string bic, decimal saldo = 0, string name = "")
        {
            Owner = owner;
            Id = id;
            Number = number;
            BIC = bic;
            Saldo = saldo;
            Name = name;
        }
        #endregion
    }
}
