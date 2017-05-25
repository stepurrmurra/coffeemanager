using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Коллекция, содержащая элементы для отображения в таблице
        /// </summary>
        public ObservableCollection<CoffeeGrade> ProductsViewSource { get; }

        /// <summary>
        /// Коллекция, содержащая все элементы модели с информацией о их видимости в таблице
        /// </summary>
        private readonly List<CoffeeGradeViewData> ProductsData = new List<CoffeeGradeViewData>();    

        private readonly DataManager dataManager = new DataManager();

        public MainWindow()
        {
            InitializeComponent();
            ProductsViewSource = new ObservableCollection<CoffeeGrade>();
        }

        /// <summary>
        /// Изменяет коллекцию ProductsViewSource в зависимости от настроек отображения ProductsData
        /// </summary>
        public void InvalidateProductsData()
        {
            ProductsViewSource.Clear();
            foreach (var prod in ProductsData)
            {
                if (prod.Visible)
                    ProductsViewSource.Add(prod.Grade);
            }
        }

        /// <summary>
        /// Применяет фильтр к таблице
        /// </summary>
        public void ApplyDataFilter(Predicate<CoffeeGrade> predicate)
        {
            foreach (var prod in ProductsData)
            {
                prod.Visible = predicate(prod.Grade);
            }
            InvalidateProductsData();
        }

        /// <summary>
        /// Вызывается при загрузке окна.
        /// Загружает данные из файла
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IList<CoffeeGrade> data;
            if (!dataManager.TryLoad(out data))
            {
                dataManager.LoadSample(out data);
            }
            foreach (var grade in data)
                ProductsData.Add(new CoffeeGradeViewData(grade));

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

        /// <summary>
        /// Нажатие кнопки "сохранить"
        /// </summary>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dataManager.Save(ProductsData.Select(p => p.Grade)))
                MessageBox.Show(dataManager.ErrorInfo, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show("Файл сохранен", "OK", MessageBoxButton.OK);
        }

        /// <summary>
        /// Определяет логику валидации ячеек после редактирования
        /// </summary>
        private void coffeeDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var grid = (DataGrid) sender;
            string text = (e.EditingElement as TextBox)?.Text;

            if (text.IndexOf(DataManager.CsvSeparator) != -1)
            {
                e.Cancel = true;
                MessageBox.Show($"Недопустимый символ '{DataManager.CsvSeparator}' в ячейке", "Недопустимый символ");
                return;
            }

            int value;
            if (int.TryParse(text, out value))
            {
                if (value >= 0)
                    return;
                e.Cancel = true;
                MessageBox.Show("Численное значение должно быть неотрицательным", "Недопустимое значение");
            }
        }

        /// <summary>
        /// Текст подсказки в поле для поиска
        /// </summary>
        public const string SearchPlaceholder = "Поиск...";

        /// <summary>
        /// Убирает подсказку на поле для поиска при нажатии
        /// </summary>
        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == SearchPlaceholder)
                searchTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Показывает подсказку на поле для поиска при потере фокуса
        /// </summary>
        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == string.Empty)
                searchTextBox.Text = SearchPlaceholder;
        }

        /// <summary>
        /// Определяет, подходит ли grade под фильтр с заданной substr
        /// </summary>
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
                    cellString = grade.Region;
                    break;
                case 3:
                    cellNumber = grade.Height;
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

        /// <summary>
        /// Запуск фильтрации при изменении строки поиска
        /// </summary>
        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var txt = searchTextBox.Text.ToLower();
            Predicate<CoffeeGrade> filter = item => true;
            if (txt != string.Empty)
                filter = (item => searchPredicate(txt, item));
            ApplyDataFilter(filter);
        }

        /// <summary>
        /// Реализует логику добавления ного элемента в таблицу
        /// </summary>
        private void coffeeDataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var grade = new CoffeeGrade();
            e.NewItem = grade;
            var viewData = new CoffeeGradeViewData(grade);
            ProductsData.Add(viewData);
        }
    }
}
