using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace NikitchenkoCSharp04 
{
    class UserEditRegisterViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _surname;
        private string _email;
        private DateTime _birthDate = DateTime.Today;
        private RelayCommand _signInCommand;
        private readonly Action<User> _onRegisterAction;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }

        public string BirthDateText { get; set; }

        public RelayCommand RegisterCmd
        {
            get
            {
                return _signInCommand ?? (_signInCommand = new RelayCommand(RegisterUser,
                           o => !string.IsNullOrWhiteSpace(_name) &&
                                !string.IsNullOrWhiteSpace(_surname) &&
                                !string.IsNullOrWhiteSpace(_email) &&
                                !string.IsNullOrWhiteSpace(BirthDateText)
                                ));
            }
        }

        private async void RegisterUser(object o)
        {
            User user = null;
            await Task.Run((() =>
            {
                try
                {
                    user = new User(_name, _surname, _email, _birthDate);
                }
                catch (UserCreationException e)
                {
                    MessageBox.Show(e.Message);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Помилка при зберіганні профілю: {e}");
                }
            }));
            if (user != null)
            {
                _onRegisterAction(user);
            }
        }

        internal UserEditRegisterViewModel(User user, Action<User> onRegisterAction)
        {
            if (user != null)
            {
                Name = user.Name;
                Surname = user.Surname;
                Email = user.Email;
                BirthDate = user.Birthday;
            }
            _onRegisterAction = onRegisterAction;
        }

       
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}