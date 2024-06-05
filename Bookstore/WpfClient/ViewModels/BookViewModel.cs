using Common.Model;
using Common.Log;
using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Data;
using WpfClient.Misc;
using System.Windows.Controls;
using WpfClient.Properties;
using System.Windows.Input;
using WpfClient.Misc;
using WpfClient.Classes;

namespace WpfClient.ViewModels
{
    internal class BookViewModel : ViewModelBase
    {
        public ICollectionView BookList { get; set; }
        public List<Author> AuthorList { get; set; }
        public string BookNameTextBox { get; set; }
        public string AuthorTextBox { get; set; }

        // Commands
        public Command NewAuthorCommand { get; set; }
        public Command NewBookCommand { get; set; }
        public Command EditBookCommand { get; set; }
        public Command DuplicateCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command LeaseCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command ReturnCommand { get; set; }

        // Undo/redo
        public Command UndoCommand { get; set; }
        public Command RedoCommand { get; set; }
        private Classes.ActionHistory history;

        private Book selectedBook;
        private List<Book> books;
        private bool isAdmin = false;

        public BookViewModel()
        {
            history = new Classes.ActionHistory();

            NewAuthorCommand = new Command(NewAuthor, IsAdmin);
            NewBookCommand = new Command(NewBook, IsAdmin);
            EditBookCommand = new Command(EditBook, IsAdmin);
            DuplicateCommand = new Command(DuplicateBook, CanDuplicate);
            DeleteCommand = new Command(DeleteBook, IsAdmin);
            RefreshCommand = new Command(RefreshList);
            LeaseCommand = new Command(LeaseBook, CanLease);
            ReturnCommand = new Command(ReturnBook, CanReturn);
            SearchCommand = new Command(FilterBooks);
            UndoCommand = new Command(Undo, history.CanUndo);
            RedoCommand = new Command(Redo, history.CanRedo);

            RefreshList();
        }

        public Book SelectedBook
        {
            get { return selectedBook; }
            set
            {
                selectedBook = value;
                DuplicateCommand.RaiseCanExecuteChanged();
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedBook");
            }
        }

        public void NewAuthor()
        {
            var win = new NewAuthorWindow();
            NewAuthorViewModel vm = (NewAuthorViewModel)win.DataContext;
            bool? result = win.ShowDialog();
            if (result == true)
            {
                var sessionService = SessionService.Instance;
                sessionService.Session.BookstoreService.CreateAuthor(vm.FirstName, vm.LastName, vm.ShortDesc, sessionService.Token);
            }
        }

        private void NewBook()
        {
            var win = new NewBookWindow(AuthorList);
            NewBookViewModel vm = (NewBookViewModel)win.DataContext;

            bool? result = win.ShowDialog();
            if(result == true)
            {
                var sessionService = SessionService.Instance;
                sessionService.Session.BookstoreService.CreateBook(vm.BookName, int.Parse(vm.PublicationYear), vm.SelectedAuthor.AuthorId, sessionService.Token);
            }

            //if (Classes.Session.Current.LibraryProxy.CreateBook(vm.BookName, vm.Author, int.Parse(vm.PublicationYear)))
            //    ClientLogger.Log($"Book {vm.BookName} successfully created.", LogLevel.INFO);
            //else
            //    ClientLogger.Log($"Book {vm.BookName} could not be created.", LogLevel.ERROR);

            RefreshList();
        }

        private void EditBook()
        {
            var win = new NewBookWindow(AuthorList);
            NewBookViewModel vm = (NewBookViewModel)win.DataContext;

            bool? result = win.ShowDialog();

            if(result == true)
            {
                var sessionService = SessionService.Instance;
                sessionService.Session.BookstoreService.EditBook(selectedBook.BookId, vm.BookName, int.Parse(vm.PublicationYear), vm.SelectedAuthor.AuthorId, sessionService.Token);
            }

            //if (Classes.Session.Current.LibraryProxy.EditBook(selectedBook, token))
            //ClientLogger.Log($"Book {vm.BookName} successfully edited.", LogLevel.INFO);
            //else
            //ClientLogger.Log($"Book {vm.BookName} could not be edited.", LogLevel.ERROR);

            RefreshList();
        }

        private void DuplicateBook()
        {
            var sessionService = SessionService.Instance;
            sessionService.Session.BookstoreService.CloneBook(SelectedBook, sessionService.Token);
            //                                                              izvuci username iz sesije
            //ClientLogger.Log($"Book {b.Title} duplicated.", LogLevel.INFO, );
            //Classes.Session.Current.LibraryProxy.DuplicateBook(b);
            RefreshList();
        }

        private void DeleteBook()
        {
            var sessionService = SessionService.Instance;
            string token = sessionService.Token;
            sessionService.Session.BookstoreService.DeleteBook(SelectedBook, sessionService.Token);
            //if (Classes.Session.Current.LibraryProxy.DeleteBook(selectedBook, token))
            //ClientLogger.Log($"Book {selectedBook.Title} deleted successfully.", Common.LogLevel.INFO);
            //else
            //ClientLogger.Log($"Book {selectedBook.BookName} could not be deleted.", Common.LogLevel.ERROR);

            RefreshList();
        }

        private bool CanDuplicate()
        {
            return SelectedBook != null && IsAdmin();
        }

        private void RefreshList()
        {
            using (var c = new WaitCursor())
            {
                var session = SessionService.Instance.Session;
                Dictionary<int, Book> booksDictionary = session.BookstoreService.GetBooks();
                books = booksDictionary.Values.ToList();

                AuthorList = session.BookstoreService.GetAuthors();

                CollectionViewSource itemSourceList = new CollectionViewSource() { Source = books };
                BookList = itemSourceList.View;

                bool AdminStatus = SessionService.Instance.Session.BookstoreService.GetMemberInfo(SessionService.Instance.Token).IsAdmin;
                isAdmin = AdminStatus;
                NewAuthorCommand.RaiseCanExecuteChanged();
                NewBookCommand.RaiseCanExecuteChanged();
                EditBookCommand.RaiseCanExecuteChanged();
                DuplicateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();

                OnPropertyChanged(nameof(BookList));
            }
        }

 
        private void LeaseBook()
        {
            var sessionService = SessionService.Instance;
            if(sessionService.Session.BookstoreService.LeaseBook(selectedBook, sessionService.Token))
            {
                RefreshList();
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
            }

            Action redo = () =>
            {
                sessionService.Session.BookstoreService.ReturnBook(SelectedBook, sessionService.Token);

                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedBook");
                RefreshList();
            };
            Action undo = () =>
            {
                sessionService.Session.BookstoreService.LeaseBook(SelectedBook, sessionService.Token);

                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedBook");
                RefreshList();
            };

            history.AddAndExecute(new RevertableCommand(redo, undo));
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();

            //ClientLogger.Log($"{Classes.Session.Current.LoggedInUser} leased book {selectedBook.Title}", Common.LogLevel.INFO);
        }

        private bool CanLease()
        {
            return selectedBook != null && selectedBook.Username == null;
        }

        private bool IsAdmin()
        {
            return isAdmin;
        }

        private void ReturnBook()
        {
            var sessionService = SessionService.Instance;
            if(sessionService.Session.BookstoreService.ReturnBook(selectedBook, sessionService.Token))
            {
                RefreshList();
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
            }
            
        }

        private bool CanReturn()
        {
            var sessionService = SessionService.Instance;
            var bookUsername = sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username;
            return selectedBook != null && selectedBook.Username != null && selectedBook.Username == bookUsername;
        }

        private void FilterBooks()
        {
            BookList.Filter = new Predicate<object>(b =>
                ((Book)b).Title.Contains(BookNameTextBox == null ? "" : BookNameTextBox) &&
                ((Book)b).Author.FirstName.Contains(AuthorTextBox == null ? "" : AuthorTextBox)
            );
        }

        private void Undo()
        {
            history.Undo();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void Redo()
        {
            history.Redo();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }
    }
}
