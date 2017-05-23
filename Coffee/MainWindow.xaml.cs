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

namespace Coffee
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<CoffeeGrade> productsData;
        private readonly DataManager dataManager = new DataManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!dataManager.TryLoad(out productsData))
            {
                dataManager.LoadSample(out productsData);
            }

            coffeeDataGrid.ItemsSource = productsData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            productsData.RemoveAt(0);
            coffeeDataGrid.Items.Refresh();
            return;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!dataManager.Save(productsData))
                MessageBox.Show(dataManager.ErrorInfo, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
