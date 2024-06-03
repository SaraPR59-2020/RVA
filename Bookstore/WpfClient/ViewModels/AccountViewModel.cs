using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    internal class AccountViewModel : ViewModelBase
    {
        public string FirstNameTextBox { get; set; }
        public string LastNameTextBox { get; set; }

        public Command SaveCommand { get; set; }



        public AccountViewModel()
        {
            //UserInfo ui = Session.Current.LibraryProxy.GetUserInfo(Session.Current.LoggedInUser);
            //FirstNameTextBox = ui.FirstName;
            //LastNameTextBox = ui.LastName;

            //SaveCommand = new Command(SaveAccountChanges);
        }

        private void SaveAccountChanges()
        {
            //Session.Current.LibraryProxy.EditUserInfo(Session.Current.LoggedInUser, FirstNameTextBox, LastNameTextBox);
            //ClientLogger.Log($"Account {Session.Current.LoggedInUser} successfully edited.", Common.LogLevel.Info);
        }
    }
}
