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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ykupova_miracle
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }
        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Meneger.MainFrame.Navigate(new ProductPage(null));
            MessageBox.Show("Вошли как гость");
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = TBLogin.Text;
            if (login == "")
            {
                MessageBox.Show("У вас есть пустые поля!");
                return;
            }
            Client user = YakupovaMiracleEntities.GetContext().Client.ToList().Find(p => p.Login == login);
            if (user != null)
            {
                Meneger.MainFrame.Navigate(new ProductPage(user));
                TBLogin.Text = "";
                MessageBox.Show("Вы успешно вошли в акaунт!");
            }
            else
            {
                MessageBox.Show("Данные введены неверно, попробуйте еще раз!");
                return;
            }
        }
    }
}
