using CZN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CZN.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private int currentUserId;
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            var user = Helper.AuthUser(username, password);

            if (user != null)
            {
                if (user.IsLocked && user.RoleID == 1)
                {
                    MessageBox.Show("Ваш доступ как администратора ограничен!", "Ограничение доступа",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                    this.Close();
                    return;
                }

                if (Helper.IsAdmin(user))
                {
                    AdminWindow adminWindow = new AdminWindow(user.UserID);
                    adminWindow.Show();
                }
                else
                {
                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void linkGuest_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();
            userWindow.Show();
            this.Close();
        }
    }
}
