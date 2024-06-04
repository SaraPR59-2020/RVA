using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    internal class AdminViewModel : ViewModelBase
    {
        public string UsernameTextBox { get; set; }
        public string FirstNameTextBox { get; set; }
        public string LastNameTextBox { get; set; }

        public Command<object> CreateUserCommand { get; set; }

        private string errorText;



        public AdminViewModel()
        {
            UsernameTextBox = string.Empty;
            FirstNameTextBox = string.Empty;
            LastNameTextBox = string.Empty;
            CreateUserCommand = new Command<object>(CreateUser);
        }

        public string ErrorText
        {
            get { return errorText; }
            set
            {
                errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        private void CreateUser(object param)
        {
            PasswordBox pw = param as PasswordBox;

            if (!ValidateUserData(pw.Password == null ? string.Empty : pw.Password))
                return;

            var sessionService = SessionService.Instance;
            string token = sessionService.Token;
            Session session = new Session();

            //stavila sam da je admin false jer ne znam kako da proverim da li je admin ili ne - tj. da li cemo imati opciju da admin dodaje admine
            if (!session.BookstoreService.CreateUser(FirstNameTextBox, LastNameTextBox, UsernameTextBox, pw.Password, false, token))
            {
                ErrorText = "User already exists.";
                return;
            }

            UsernameTextBox = string.Empty;
            FirstNameTextBox = string.Empty;
            LastNameTextBox = string.Empty;
            pw.Password = string.Empty;
            ErrorText = string.Empty;
            OnPropertyChanged("UsernameTextBox");
            OnPropertyChanged("FirstNameTextBox");
            OnPropertyChanged("LastNameTextBox");
        }

        private bool ValidateUserData(string password)
        {
            if (UsernameTextBox == string.Empty)
            {
                ErrorText = "Username can't be empty.";
                return false;
            }

            if (password == string.Empty)
            {
                ErrorText = "Password can't be empty.";
                return false;
            }

            if (!Regex.IsMatch(FirstNameTextBox, @"^[a-zA-Z]+$"))
            {
                ErrorText = "First name must contain only letters.";
                return false;
            }

            if (!Regex.IsMatch(LastNameTextBox, @"^[a-zA-Z]+$"))
            {
                ErrorText = "Last name must contain only letters.";
                return false;
            }

            return true;
        }
    }
}
