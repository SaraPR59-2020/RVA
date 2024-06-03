using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    internal class BookViewModel : ViewModelBase
    {
        private string bookName;
        private string author;
        private string publicationYear;
        private string errorMessage;

        #region Properties
        public string BookName
        {
            get { return bookName; }
            set
            {
                bookName = value;
                OnPropertyChanged("BookName");
            }
        }

        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                OnPropertyChanged("Author");
            }
        }

        public string PublicationYear
        {
            get { return publicationYear; }
            set
            {
                publicationYear = value;
                OnPropertyChanged("PublicationYear");
            }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
        #endregion

        public Command<Window> NewBookCommand { get; set; }



        public BookViewModel()
        {
            BookName = string.Empty;
            Author = string.Empty;
            PublicationYear = string.Empty;
            ErrorMessage = string.Empty;

            NewBookCommand = new Command<Window>(NewBook);
        }

        private void NewBook(Window window)
        {
            if (!ValidateBook())
                return;

            window.Close();
        }

        private bool ValidateBook()
        {
            if (BookName == string.Empty)
            {
                ErrorMessage = "Book name cannot be empty.";
                return false;
            }

            if (Author == string.Empty)
            {
                ErrorMessage = "Author name cannot be empty.";
                return false;
            }

            if (!Regex.IsMatch(PublicationYear, @"^[1-9][0-9]{3}$"))
            {
                ErrorMessage = "Publication year must be a valid year.";
                return false;
            }

            ErrorMessage = string.Empty;

            return true;
        }
    }
}
