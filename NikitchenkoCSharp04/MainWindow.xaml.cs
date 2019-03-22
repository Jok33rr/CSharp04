using System;
using System.Windows;


namespace NikitchenkoCSharp04 { 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new UserListViewModel(delegate () { Dispatcher.Invoke(UsersDataGrid.Items.Refresh); });
        }

        
    }
}
