using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public string Username { get; set; }
        public Command<object> LoginCommand { get; set; }
        private string errorText;
        public string ErrorText
        {
            get { return errorText; }
            set
            {
                errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        public LoginViewModel()
        {
            LoginCommand = new Command<object>(Login);
            var sessionService = SessionService.Instance;
        }

        private void Login(object parameter)
        {
            PasswordBox pw = parameter as PasswordBox;

            var sessionService = SessionService.Instance;
            string token = sessionService.Session.BookstoreService.LogIn(Username, pw.Password);
            if(token != null)
            {
                sessionService.Token = token;
            }
            else
            {
                ErrorText = "Incorrect credentials or already logged in!";
            }
        }
    }
}
