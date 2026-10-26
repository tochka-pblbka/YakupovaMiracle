using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Product> CurrentPageList = new List<Product>();
        List<Product> TableList;
        private Client _currentUser;
        private int UserRoles;
        public ProductPage(Client user)
        {
            InitializeComponent();
            _currentUser = user;
            UserRoles = user?.RoleID ?? 0;
            var currentProduct = YakupovaMiracleEntities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
            ComboCateghory.SelectedIndex = 0;
            ComboBoh.SelectedIndex = 0;
            UpdateProduct();
            if (user != null)
            {
                FIOTB.Text = "Вы авторизованы как " + user.FirstName + " " +
                    user.LastName + " " + user.Patrynomic;
                switch (user.RoleID)
                {
                    case 1:
                        RoleTB.Text = "Роль: Администратор"; break;
                    case 2:
                        RoleTB.Text = "Роль: Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Роль: Авторизованный клиент"; break;
                }
            }
            else
            {
                FIOTB.Text = "Вы вошли как гость";
                RoleTB.Text = "Роль: Гость";
            }
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            if (PageListBox == null || TableList == null) return;

            double PageSize = 10;
            CountRecords = TableList.Count;
            CountPage = (int)Math.Ceiling((double)CountRecords / PageSize);

            
            if (CountPage == 0) CountPage = 1;

            if (selectedPage.HasValue)
            {
                CurrentPage = selectedPage.Value;
            }
            else
            {
                switch (direction)
                {
                    case 1: CurrentPage--; break; 
                    case 2: CurrentPage++; break; 
                }
            }

            
            if (CurrentPage < 0) CurrentPage = 0;
            if (CurrentPage >= CountPage) CurrentPage = CountPage - 1;

            
            CurrentPageList = TableList
                .Skip(CurrentPage * 10)
                .Take(10)
                .ToList();

            
            PageListBox.Items.Clear();
            for (int i = 1; i <= CountPage; i++)
            {
                PageListBox.Items.Add(i);
            }

           
            if (CurrentPage >= 0 && CurrentPage < PageListBox.Items.Count)
            {
                PageListBox.SelectedIndex = CurrentPage;
            }

           
            ProductListView.ItemsSource = CurrentPageList;
            ProductListView.Items.Refresh();

        
            TBRecordCount.Text = $"Показано {CurrentPageList.Count} из {CountRecords}";
        }



        private void UpdateRecordCount(int currentPageCount, int totalCount)
        {
            TBRecordCount.Text = $"Показано {currentPageCount} из {totalCount} записей";
        }

        private void UpdateProduct()
        {
            var currentProduct = YakupovaMiracleEntities.GetContext().Product.ToList();

            if (!string.IsNullOrEmpty(TBSearch.Text))
            {
                
                string searchText = TBSearch.Text.ToLower();

                
                searchText = searchText.Replace('ё', 'е');

                currentProduct = currentProduct.Where(p =>
                {
                    // Для каждого поля делаем то же самое: ToLower() и замена ё->е
                    bool nameMatch = p.ProductName != null &&
                                     p.ProductName.ToLower().Replace('ё', 'е').Contains(searchText);

                    bool catMatch = p.Category != null &&
                                    p.CategoryTitle.ToLower().Replace('ё', 'е').Contains(searchText);

                    bool manMatch = p.Manufacture != null &&
                                    p.ManufactureTitle.ToLower().Replace('ё', 'е').Contains(searchText);

                    bool subMatch = p.SubCategory != null &&
                                    p.SubCategoryTitle.ToLower().Replace('ё', 'е').Contains(searchText);

                    return nameMatch || catMatch || manMatch || subMatch;
                }).ToList();
            }

            if (ComboBoh.SelectedIndex > 0)
            {
                string sortType = (ComboBoh.SelectedItem as TextBlock).Text;

                switch (sortType)
                {
                    case "по нименованию от А до я":
                        currentProduct = currentProduct.OrderBy(p => p.ProductName).ToList();
                        break;

                    case "По возрастнаию цены":
                        currentProduct = currentProduct.OrderBy(p => p.Price).ToList();
                        break;
                    case "По убыванию цены":
                        currentProduct = currentProduct.OrderByDescending(p => p.Price).ToList();
                        break;

                }
            }

            if (ComboCateghory.SelectedIndex == 1)
            {
                currentProduct = currentProduct.Where(p => p.CategoryTitle == "Детская обувь").ToList();
            }
            if (ComboCateghory.SelectedIndex == 2)
            {
                currentProduct = currentProduct.Where(p => p.CategoryTitle == "Женская обувь").ToList();
            }
            if (ComboCateghory.SelectedIndex == 3)
            {
                currentProduct = currentProduct.Where(p => p.CategoryTitle == "Мужская обувь").ToList();
            }

            TableList = currentProduct;
            ChangePage(0, 0);
        }

      
        private void TBSearch_TextChanged(object sender, TextChangedEventArgs e) => UpdateProduct();
        private void ComboCateghory_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateProduct();
        private void ComboBoh_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateProduct();

        private void LeftDirBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PageListBox.SelectedItem != null)
            {
              
                int selected = Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1;
                ChangePage(0, selected);
            }
        }

       
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                var context = YakupovaMiracleEntities.GetContext();
                foreach (var entry in context.ChangeTracker.Entries().ToList())
                {
                    entry.State = EntityState.Detached;
                }
                UpdateProduct();
            }
        }

    }
}