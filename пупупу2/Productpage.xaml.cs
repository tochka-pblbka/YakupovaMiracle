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

namespace Yakupova_Miracle
{
    /// <summary>
    /// Логика взаимодействия для Productpage.xaml
    /// </summary>
    public partial class Productpage : Page
    {
        List<Product> CurrentPageList = new List<Product>();
        List<Product> TableList;

        private Client _currentUser;
        private int UserRoles;

        public Productpage(Client user)
        {
            InitializeComponent();
            var currentProduct = YakupovaMiracleEntities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
            ComboCategory.SelectedIndex = 0;
            ComboFiltr.SelectedIndex = 0;
            ProdutUpdate();

            _currentUser = user;
            UserRoles = user?.RoleID ?? 0;
            

            if (user != null)
            {
                FIOTB.Text = "Вы авторизованы как " + user.FirstName + " " +
                    user.LastName + " " + user.Patrynomic;
                switch (user.RoleID)
                {
                    case 1: // Администратор
                        RoleTB.Text = "Роль: Администратор";
                        BtnCart.Visibility = Visibility.Visible;
                        BtnOrders.Visibility = Visibility.Visible;
                        break;
                    case 2: // Менеджер
                        RoleTB.Text = "Роль: Менеджер";
                       BtnCart.Visibility = Visibility.Visible;
                        BtnOrders.Visibility = Visibility.Visible;
                        break;
                    case 3: // Клиент
                        RoleTB.Text = "Роль: Авторизованный клиент";
                        BtnCart.Visibility = Visibility.Visible;
                        BtnOrders.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            else
            {
                FIOTB.Text = "Вы вошли как гость";
                RoleTB.Text = "Роль: Гость";
                BtnCart.Visibility = Visibility.Collapsed;
                BtnOrders.Visibility = Visibility.Collapsed;
            }

          

        }

        private void ProdutUpdate()
        {
           var currenyProduct = YakupovaMiracleEntities.GetContext().Product.ToList();
            if(!string.IsNullOrEmpty(TbSearche.Text))
            {
                string searcheText = TbSearche.Text.ToLower().Replace("ё", "е");
                currenyProduct = currenyProduct.Where(p=>
                
                    (p.ProductName != null && p.ProductName.ToLower().Replace("ё", "е").Contains(searcheText))
                    || (p.Compound != null && p.Compound.ToLower().Replace("ё", "е").Contains(searcheText))
                ).ToList();
            }
            
            if (ComboFiltr.SelectedIndex > 0)
            {
                string sortType = (ComboFiltr.SelectedItem as TextBlock).Text;
                switch(sortType)
                {
                    case "По увеличению цены":
                        currenyProduct = currenyProduct.OrderBy(p => p.Price).ToList();
                        break;
                    case "По уменьшению цены":
                        currenyProduct = currenyProduct.OrderByDescending(p => p.Price).ToList();
                        break;
                }
            }

            if (ComboCategory.SelectedIndex == 1)
            {
                currenyProduct = currenyProduct.Where(p => p.CategoryName == "Детская обувь").ToList();
            }
            if (ComboCategory.SelectedIndex == 2)
            {
                currenyProduct = currenyProduct.Where(p => p.CategoryName == "Женская обувь").ToList();
            }
            if (ComboCategory.SelectedIndex == 3)
            {
                currenyProduct = currenyProduct.Where(p => p.CategoryName == "Мужская обувь").ToList();
            }
            
            ProductListView.ItemsSource = currenyProduct;
            TBRecordCount.Text = $"Показано записей: {currenyProduct.Count}";
        }

        private void TbSearche_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProdutUpdate();
        }

        private void ComboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProdutUpdate();
        }

        private void ComboFiltr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProdutUpdate();
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage(_currentUser));
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(_currentUser));
        }
    }
}
