using CZN.Models;
using CZN.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CZN.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private List<AdminEmployeesModel> _allEmployees;

        public AdminWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            _allEmployees = Helper.GetEmployeesWithDetails();
            dgEmployees.ItemsSource = _allEmployees;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = txtSearch.Text.ToLower();
            dgEmployees.ItemsSource = _allEmployees.Where(emp =>
                emp.LastName.ToLower().Contains(searchText) ||
                emp.FirstName.ToLower().Contains(searchText) ||
                emp.MiddleName.ToLower().Contains(searchText) ||
                emp.Department.ToLower().Contains(searchText) ||
                emp.Position.ToLower().Contains(searchText));
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EmployeeEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var employee = (sender as Button).DataContext as AdminEmployeesModel;
            var editWindow = new EmployeeEditWindow(employee);
            if (editWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var employee = (sender as Button).DataContext as AdminEmployeesModel;
            if (MessageBox.Show($"Вы точно хотите удалить сотрудника {employee.LastName}?",
                               "Подтверждение удаления",
                               MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (Helper.DeleteEmployee(employee.EmployeeID))
                {
                    MessageBox.Show("Сотрудник успешно удален");
                    LoadEmployees();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить сотрудника", "Ошибка");
                }
            }
        }
    }
}
