using System;
using System.Windows;

namespace NikitchenkoCSharp04 
{

    public partial class CreateEditUserWindow : Window
    {
        public CreateEditUserWindow(Action<User> onRegisterAction, User user = null)
        {
            InitializeComponent();
            DataContext = new UserEditRegisterViewModel(user, delegate (User newUser)
            {
                Close();
                onRegisterAction(newUser);
            });
        }
    }
}
