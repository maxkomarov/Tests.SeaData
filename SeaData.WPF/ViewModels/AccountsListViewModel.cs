using System.Collections.ObjectModel;

namespace SeaData.WPF.ViewModels
{
    public class AccountsListViewModel
    {
        private ObservableCollection<CustomerViewModel> accounts;
        private CustomerViewModel currentAccount;

        public ObservableCollection<CustomerViewModel> Accounts
        {
            get => accounts;
        }

        public CustomerViewModel CurrentAccounts
        {
            get => currentAccount;
        }
    }
}
