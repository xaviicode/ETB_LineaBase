using LineaBaseETB_V2.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LineaBaseETB_V2.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void PatBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Pat = ((PasswordBox)sender).Password;
            }
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;

            var selectedItem = dataGrid.SelectedItem;
            if (selectedItem != null)
            {
                // Aquí puedes manejar la lógica cuando se selecciona un Work Item
                // Por ejemplo, mostrar detalles, habilitar botones, etc.
                // Ejemplo simple: mostrar el Id en un mensaje (solo para prueba)
                // MessageBox.Show($"Work Item seleccionado: {selectedItem}");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

