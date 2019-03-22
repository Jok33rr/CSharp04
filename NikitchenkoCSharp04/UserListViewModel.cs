using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;


namespace NikitchenkoCSharp04
{
    class UserListViewModel : INotifyPropertyChanged
    {
        private List<User> _usersList;
        private User _selectedUser;
        private readonly Action _refreshUsersAction;
        private string _filterQuery;
        private bool _sortingAsc = true;

        private RelayCommand _deleteCmd;
        private RelayCommand _editCmd;
        private RelayCommand _registerCmd;
        private RelayCommand _sortCmd;
        private RelayCommand _clearFilterCmd;

        private static CollectionView _sortFilterParamsCollection;

        public string FilterSortByParam { get; set; }

        public static CollectionView SortFilterParams => _sortFilterParamsCollection ??(_sortFilterParamsCollection = new CollectionView(SorterByParam.FilterSortParams));

        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                _filterQuery = value;
                SelectedUser = null;
                UpdateUsersGrid();
            }
        }

        public List<User> UsersShowList =>
            (string.IsNullOrEmpty(FilterSortByParam) || string.IsNullOrEmpty(FilterQuery))
                ? _usersList
                : _usersList.FilterByParam(FilterSortByParam, FilterQuery);

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                if (_selectedUser == null) return;
            }
        }

        public RelayCommand DeleteCmd =>
            _deleteCmd ?? (_deleteCmd = new RelayCommand(DeleteUser, o => _selectedUser != null));

        private async void DeleteUser(object o)
        {
            await Task.Run((() =>
            {
                _usersList.Remove(SelectedUser);
                UpdateUsersGrid();
            }));
        }

        public RelayCommand EditCmd =>
            _editCmd ?? (_editCmd = new RelayCommand(EditImpl, o => _selectedUser != null));

        private void EditImpl(object o)
        {
            var userToEdit = _selectedUser;
            var editWindow = new CreateEditUserWindow(delegate (User edited)
            {
                userToEdit.CopyUserFrom(edited);
                UpdateUsersGrid();
            }, _selectedUser);
            editWindow.Show();
        }

        public RelayCommand RegisterCmd =>
            _registerCmd ?? (_registerCmd = new RelayCommand(RegisterUser, o => true));

        private void RegisterUser(object o)
        {
            var registerWindow = new CreateEditUserWindow(delegate (User newUser)
            {
                UsersShowList.Add(newUser);
                UpdateUsersGrid();
            });
            registerWindow.Show();
        }

        public RelayCommand SortCmd =>
            _sortCmd ?? (_sortCmd =
                new RelayCommand(SortUser, o => !string.IsNullOrEmpty(FilterSortByParam)));

        private async void SortUser(object o)
        {
            await Task.Run(() =>
            {
                _usersList = _usersList.SortByParam(FilterSortByParam, _sortingAsc);
                _sortingAsc = !_sortingAsc;
                UpdateUsersGrid();
            });
        }

        public RelayCommand ClearFilterCmd =>
            _clearFilterCmd ?? (_clearFilterCmd = new RelayCommand((o) =>
            {
                FilterQuery = "";
                OnPropertyChanged($"FilterQuery");
            },
                o => !string.IsNullOrEmpty(FilterQuery)));

        private void UpdateUsersGrid()
        {
            User.SaveData(_usersList);
            OnPropertyChanged($"UsersShowList");
            _refreshUsersAction();
        }

        public UserListViewModel(Action updateGridItems)
        {
            _refreshUsersAction = updateGridItems;
            _usersList = new List<User>();
            User.LoadAllInto(UsersShowList, UpdateUsersGrid);
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}