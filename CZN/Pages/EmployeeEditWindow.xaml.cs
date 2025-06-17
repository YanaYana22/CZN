using CZN.Models;
using CZN.Services;
using System.Linq;
using System.Windows;

namespace CZN.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeeEditWindow.xaml
    /// </summary>
    public partial class EmployeeEditWindow : Window
    {
        private AdminEmployeesModel _employee;

        public EmployeeEditWindow(AdminEmployeesModel employee = null)
        {
            InitializeComponent();

            _employee = employee ?? new AdminEmployeesModel();
            DataContext = _employee;

            cbDepartments.ItemsSource = Helper.GetContext().Departments.ToList();
            cbPositions.ItemsSource = Helper.GetContext().Positions.ToList();

            if (!string.IsNullOrEmpty(_employee.Department))
                cbDepartments.SelectedItem = cbDepartments.ItemsSource
                    .Cast<dynamic>()
                    .FirstOrDefault(d => d.Name == _employee.Department);

            if (!string.IsNullOrEmpty(_employee.Position))
                cbPositions.SelectedItem = cbPositions.ItemsSource
                    .Cast<dynamic>()
                    .FirstOrDefault(p => p.Title == _employee.Position);

            chkIsAdmin.Checked += (s, e) => pnlAdminCredentials.Visibility = Visibility.Visible;
            chkIsAdmin.Unchecked += (s, e) => pnlAdminCredentials.Visibility = Visibility.Collapsed;
            pnlAdminCredentials.Visibility = _employee.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_employee.LastName) ||
                cbDepartments.SelectedItem == null ||
                cbPositions.SelectedItem == null)
            {
                MessageBox.Show("Заполните обязательные поля (Фамилия, Отдел, Должность)");
                return;
            }

            _employee.Department = ((dynamic)cbDepartments.SelectedItem).Name;
            _employee.Position = ((dynamic)cbPositions.SelectedItem).Title;

            string password = null;
            if (_employee.IsAdmin)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Для администратора необходимо указать логин");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Password) && string.IsNullOrEmpty(_employee.Username))
                {
                    MessageBox.Show("Для администратора необходимо указать пароль");
                    return;
                }

                password = txtPassword.Password;
            }

            if (Helper.SaveEmployee(_employee, _employee.IsAdmin ? txtUsername.Text : null, password))
            {
                DialogResult = true;
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
