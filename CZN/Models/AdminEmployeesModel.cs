using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CZN.Models
{
    public class AdminEmployeesModel : INotifyPropertyChanged
    {
        private bool _isAdmin;
        private bool _isLocked;
        private string _username;
        private int _userId;
        private int _currentAdminId;

        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Department { get; set; }
        public string DepartmentAddress { get; set; }
        public string DistrictName { get; set; }
        public string Position { get; set; }
        public string InternalPhone { get; set; }
        public string CityPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }

        public int UserID
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(CanBeLocked));
                }
            }
        }

        public int CurrentAdminId
        {
            get => _currentAdminId;
            set
            {
                if (_currentAdminId != value)
                {
                    _currentAdminId = value;
                    NotifyPropertyChanged(nameof(CanBeLocked));
                }
            }
        }

        public bool CanBeLocked => UserID != 0 && UserID != CurrentAdminId;

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(CanBeLocked));

                    if (!value)
                    {
                        Username = null;
                        IsLocked = false;
                    }
                }
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked != value && (UserID != CurrentAdminId || !value))
                {
                    _isLocked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}