using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<CoffeeGrade> ProductsData { get; }

        private readonly DataManager dataManager = new DataManager();

        public MainWindow()
        {
            InitializeComponent();
            ProductsData = new ObservableCollection<CoffeeGrade>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IList<CoffeeGrade> data;
            if (!dataManager.TryLoad(out data))
            {
                dataManager.LoadSample(out data);
            }
            foreach (var grade in data)
                ProductsData.Add(grade);

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
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dataManager.Save(ProductsData))
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

        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
