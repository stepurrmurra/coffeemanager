using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Coffee
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<CoffeeGrade> ProductsViewData { get; }
        private readonly List<HidableCoffeeGrade> ProductsData = new List<HidableCoffeeGrade>();    

        private readonly DataManager dataManager = new DataManager();

        public MainWindow()
        {
            InitializeComponent();
            ProductsViewData = new ObservableCollection<CoffeeGrade>();
        }

        public void InvalidateProductsData()
        {
            ProductsViewData.Clear();
            foreach (var prod in ProductsData)
            {
                if (prod.Visibility)
                    ProductsViewData.Add(prod.Grade);
            }
        }

        public void ApplyDataFilter(Predicate<CoffeeGrade> predicate)
        {
            foreach (var prod in ProductsData)
            {
                prod.Visibility = predicate(prod.Grade);
            }
            InvalidateProductsData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IList<CoffeeGrade> data;
            if (!dataManager.TryLoad(out data))
            {
                dataManager.LoadSample(out data);
            }
            foreach (var grade in data)
                ProductsData.Add(new HidableCoffeeGrade(grade));

            DataContext = this;

            foreach (var column in coffeeDataGrid.Columns)
            {
                var item = new ComboBoxItem
                {
                    Content = column.Header.ToString()
                };
                searchComboBox.Items.Add(item);
            }
            searchComboBox.SelectedIndex = 0;
            InvalidateProductsData();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dataManager.Save(ProductsData.Select(p => p.Grade)))
                MessageBox.Show(dataManager.ErrorInfo, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void coffeeDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var grid = (DataGrid) sender;
            string text = e.EditingElement.ToString();

            if (text.IndexOf(DataManager.CsvSeparator) != -1)
            {
                e.Cancel = true;
                MessageBox.Show($"Недопустимый символ '{DataManager.CsvSeparator}' в ячейке", "Недопустимый символ");
            }
        }

        public const string SearchPlaceholder = "Поиск...";

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == SearchPlaceholder)
                searchTextBox.Text = string.Empty;
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == string.Empty)
                searchTextBox.Text = SearchPlaceholder;
        }

        private bool searchPredicate(string substr, CoffeeGrade grade)
        {
            if (grade == null)
                return false;

            bool isNumber = false;
            string cellString = string.Empty;
            int cellNumber = 0;

            switch (searchComboBox.SelectedIndex)
            {
                case 0:
                    cellString = grade.Name;
                    break;
                case 1:
                    cellString = grade.FullName;
                    break;
                case 2:
                    cellNumber = grade.MinHeight;
                    isNumber = true;
                    break;
                case 3:
                    cellNumber = grade.MaxHeight;
                    isNumber = true;
                    break;
                case 4:
                    cellNumber = grade.RipeDuration;
                    isNumber = true;
                    break;
                case 5:
                    cellString = grade.Description;
                    break;
                default: throw new NotImplementedException();
            }

            if (!isNumber)
                return cellString.ToLower().IndexOf(substr) != -1;
            int searchVal;
            if (!int.TryParse(substr, out searchVal))
                return false;
            return searchVal == cellNumber;
        }

        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var txt = searchTextBox.Text.ToLower();
            Predicate<CoffeeGrade> filter = item => true;
            if (txt != string.Empty)
                filter = (item => searchPredicate(txt, item));
            ApplyDataFilter(filter);
        }
    }
}
