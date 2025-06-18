using CZN.Models;
using CZN.Services;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CZN.Pages
{
    public partial class EmployeeEditWindow : Window, INotifyPropertyChanged
    {
        private AdminEmployeesModel _employee;
        private CZNEntities1 _context;
        public event PropertyChangedEventHandler PropertyChanged;

        public EmployeeEditWindow(AdminEmployeesModel employee = null)
        {
            InitializeComponent();
            _context = new CZNEntities1();

            _employee = employee ?? new AdminEmployeesModel();
            DataContext = _employee;

            LoadComboBoxData();

            chkIsAdmin.Checked += (s, e) => pnlAdminCredentials.Visibility = Visibility.Visible;
            chkIsAdmin.Unchecked += (s, e) => pnlAdminCredentials.Visibility = Visibility.Collapsed;
            pnlAdminCredentials.Visibility = _employee.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadComboBoxData()
        {
            _context.Departments.Load();
            _context.Positions.Load();

            cbDepartments.ItemsSource = _context.Departments.Local;
            cbPositions.ItemsSource = _context.Positions.Local;

            if (!string.IsNullOrEmpty(_employee.Department))
                cbDepartments.SelectedValue = _employee.Department;

            if (!string.IsNullOrEmpty(_employee.Position))
                cbPositions.SelectedValue = _employee.Position;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdateBindings();

            if (string.IsNullOrWhiteSpace(_employee.LastName) ||
               string.IsNullOrWhiteSpace(_employee.FirstName))
            {
                MessageBox.Show("Заполните Фамилию и Имя");
                return;
            }

            if (cbDepartments.SelectedItem == null || cbPositions.SelectedItem == null)
            {
                MessageBox.Show("Выберите Отдел и Должность");
                return;
            }

            if (_employee.IsAdmin && string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Укажите логин для администратора");
                return;
            }

            try
            {
                bool result = Helper.SaveEmployee(_employee,
                    _employee.IsAdmin ? txtUsername.Text : null,
                    _employee.IsAdmin ? txtPassword.Password : null);

                if (result)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка сохранения");
                }
            }
            finally
            {
                _context?.Dispose();
            }
        }

        private void UpdateBindings()
        {
            cbDepartments.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
            cbPositions.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
        }

        private void cbDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDepartments.SelectedItem is Departments selectedDepartment)
            {
                _employee.Department = selectedDepartment.Name;
                _employee.DepartmentAddress = selectedDepartment.Address;

                var district = _context.Districts
                    .FirstOrDefault(d => d.DistrictID == selectedDepartment.DistrictID);
                _employee.DistrictName = district?.Name;

                OnPropertyChanged(nameof(_employee.Department));
                OnPropertyChanged(nameof(_employee.DepartmentAddress));
                OnPropertyChanged(nameof(_employee.DistrictName));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void Window_Closed(object sender, System.EventArgs e)
        {
            _context?.Dispose();
        }
    }
}