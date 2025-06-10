using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LineaBaseETB_V2.Models;
using LineaBaseETB_V2.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LineaBaseETB_V2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Lista de estados disponibles para el filtro de Estado (puedes ajustarla según tus necesidades)
        public ObservableCollection<string> EstadosDisponibles { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> IniciativasDisponibles { get; } = new ObservableCollection<string>();



        // Estado seleccionado por el usuario en el filtro
        private string _estadoSeleccionado = "Todos";
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    // Puedes llamar a AplicarFiltros() aquí si quieres filtrado automático
                }
            }
        }

        // Filtro de ID (permite búsqueda parcial por texto)
        private string _idFiltro;
        public string IdFiltro
        {
            get => _idFiltro;
            set
            {
                if (SetProperty(ref _idFiltro, value))
                {
                    // Puedes llamar a AplicarFiltros() aquí si quieres filtrado automático
                }
            }
        }

        // Filtro de Iniciativa (permite búsqueda parcial por texto)
        private string _iniciativaFiltro;
        public string IniciativaFiltro
        {
            get => _iniciativaFiltro;
            set
            {
                if (SetProperty(ref _iniciativaFiltro, value))
                {
                    // Puedes llamar a AplicarFiltros() aquí si quieres filtrado automático
                }
            }
        }

        // Comando para aplicar los filtros (se enlaza al botón "Filtrar" en la UI)
        public ICommand FiltrarCommand { get; }

        // Comando para limpiar todos los filtros (se enlaza al botón "Limpiar Filtros" en la UI)
        public ICommand LimpiarFiltrosCommand { get; }

        // --- Variables y propiedades para la lógica de Azure DevOps y la UI ---

        private string _organization;
        private string _pat;
        private bool _isLoading;
        private string _statusMessage;

        private AzureDevOpsService _azureService;

        // Listas para los proyectos y los Work Items consultados
        public ObservableCollection<string> Proyectos { get; } = new ObservableCollection<string>();
        public ObservableCollection<WorkItemModel> WorkItems { get; } = new ObservableCollection<WorkItemModel>();

        // Lista interna de proyectos seleccionados (en este caso, siempre serán todos)
        private List<string> _proyectosSeleccionados = new List<string>();

        // Vista filtrable de los Work Items (usada por el DataGrid)
        private ICollectionView _workItemsView;
        public ICollectionView WorkItemsView
        {
            get => _workItemsView;
            private set => SetProperty(ref _workItemsView, value);
        }

        // Brushes para la validación visual de campos en la UI
        private Brush _organizationBorderBrush = Brushes.Gray;
        public Brush OrganizationBorderBrush
        {
            get => _organizationBorderBrush;
            set => SetProperty(ref _organizationBorderBrush, value);
        }

        private Brush _patBorderBrush = Brushes.Gray;
        public Brush PatBorderBrush
        {
            get => _patBorderBrush;
            set => SetProperty(ref _patBorderBrush, value);
        }

        // Propiedades para los campos de conexión
        public string Organization
        {
            get => _organization;
            set
            {
                if (SetProperty(ref _organization, value))
                {
                    ClearData();
                    _ = LoadProjectsAsync();
                    ValidarCampos();
                    ((RelayCommand)ConsultarCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Pat
        {
            get => _pat;
            set
            {
                if (SetProperty(ref _pat, value))
                {
                    ClearData();
                    _ = LoadProjectsAsync();
                    ValidarCampos();
                    ((RelayCommand)ConsultarCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (SetProperty(ref _isLoading, value))
                    ((RelayCommand)ConsultarCommand).RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        // Indica si se puede consultar (habilita/deshabilita el botón de consulta)
        public bool CanConsultar =>
            !IsLoading &&
            !string.IsNullOrWhiteSpace(Organization) &&
            !string.IsNullOrWhiteSpace(Pat);

        // Comando para consultar los Work Items desde Azure DevOps
        public ICommand ConsultarCommand { get; }

        // Constructor del ViewModel: inicializa comandos y la vista filtrable
        public MainViewModel()
        {
            ConsultarCommand = new RelayCommand(async () => await ConsultarWorkItemsAsync(), () => CanConsultar);
            FiltrarCommand = new RelayCommand(async () => await AplicarFiltros());
            LimpiarFiltrosCommand = new RelayCommand(async () => await LimpiarFiltros());

            WorkItemsView = CollectionViewSource.GetDefaultView(WorkItems);
            WorkItemsView.Filter = null; // Sin filtro por defecto
        }

        // Limpia los datos de proyectos y Work Items
        private void ClearData()
        {
            Proyectos.Clear();
            WorkItems.Clear();
            StatusMessage = string.Empty;
        }

        // Carga todos los proyectos automáticamente después de ingresar PAT y organización
        private async Task LoadProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(Organization) || string.IsNullOrWhiteSpace(Pat))
                return;

            try
            {
                IsLoading = true;
                StatusMessage = "Cargando proyectos...";

                _azureService = new AzureDevOpsService(Organization, Pat);
                var proyectos = await _azureService.ObtenerProyectosAsync();

                Proyectos.Clear();
                foreach (var p in proyectos)
                    Proyectos.Add(p);

                // Selecciona todos los proyectos automáticamente
                _proyectosSeleccionados = proyectos.ToList();

                StatusMessage = $"Se cargaron {Proyectos.Count} proyectos.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error al cargar proyectos: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Consulta los Work Items de todos los proyectos seleccionados automáticamente
        private async Task ConsultarWorkItemsAsync()
        {
            if (_azureService == null)
            {
                StatusMessage = "Primero debe cargar los proyectos.";
                return;
            }

            if (_proyectosSeleccionados == null || _proyectosSeleccionados.Count == 0)
            {
                StatusMessage = "No hay proyectos disponibles para consultar.";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Consultando Work Items de todos los proyectos...";

                WorkItems.Clear();

                foreach (var proyecto in _proyectosSeleccionados)
                {
                    StatusMessage = $"Consultando {proyecto}...";
                    var items = await _azureService.ObtenerWorkItemsAsync(proyecto);

                    foreach (var wi in items)
                        WorkItems.Add(wi);
                }

                StatusMessage = $"Se obtuvieron {WorkItems.Count} Work Items de {Proyectos.Count} proyectos.";
                ActualizarValoresFiltros();
            }

            catch (Exception ex)
            {
                StatusMessage = $"Error al consultar Work Items: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Valida los campos de conexión y actualiza los bordes en la UI
        private void ValidarCampos()
        {
            OrganizationBorderBrush = string.IsNullOrWhiteSpace(Organization) ? Brushes.Red : Brushes.Gray;
            PatBorderBrush = string.IsNullOrWhiteSpace(Pat) ? Brushes.Red : Brushes.Gray;
        }

        /// <summary>
        /// Actualiza dinámicamente los valores de los filtros Estado e Iniciativa
        /// según los Work Items cargados.
        /// </summary>
        private void ActualizarValoresFiltros()
        {
            // Limpia las listas actuales
            EstadosDisponibles.Clear();
            IniciativasDisponibles.Clear();

            // Extrae valores únicos y ordenados de los Work Items cargados
            var estados = WorkItems.Select(wi => wi.State)
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Distinct()
                                   .OrderBy(s => s);

            var iniciativas = WorkItems.Select(wi => wi.NumeroIniciativa)
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Distinct()
                                       .OrderBy(s => s);

            // Llena las listas dinámicas
            foreach (var estado in estados)
                EstadosDisponibles.Add(estado);

            foreach (var iniciativa in iniciativas)
                IniciativasDisponibles.Add(iniciativa);
        }


        /// <summary>
        /// Aplica los filtros seleccionados a la vista de WorkItems.
        /// Si el filtro de estado es "Todos" o vacío, no filtra por estado.
        /// Si los otros filtros están vacíos, tampoco filtra por ellos.
        /// </summary>
        private async Task AplicarFiltros()
        {
            WorkItemsView.Filter = item =>
            {
                var workItem = item as WorkItemModel;
                if (workItem == null)
                    return false;

                // Filtrado por Estado
                bool estadoOk = string.IsNullOrWhiteSpace(EstadoSeleccionado) ||
                                EstadoSeleccionado == "Todos" ||
                                (workItem.State?.Equals(EstadoSeleccionado, StringComparison.OrdinalIgnoreCase) == true);

                // Filtrado por ID (permite búsqueda parcial)
                bool idOk = string.IsNullOrWhiteSpace(IdFiltro) ||
                            (workItem.Id.ToString().IndexOf(IdFiltro, StringComparison.OrdinalIgnoreCase) >= 0);

                // Filtrado por Iniciativa (permite búsqueda parcial)
                bool iniciativaOk = string.IsNullOrWhiteSpace(IniciativaFiltro) ||
                                    (!string.IsNullOrEmpty(workItem.NumeroIniciativa) &&
                                     workItem.NumeroIniciativa.IndexOf(IniciativaFiltro, StringComparison.OrdinalIgnoreCase) >= 0);

                // Solo muestra los que cumplen todos los filtros activos
                return estadoOk && idOk && iniciativaOk;
            };

            WorkItemsView.Refresh();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Limpia todos los filtros y muestra todos los Work Items.
        /// </summary>
        private async Task LimpiarFiltros()
        {
            EstadoSeleccionado = "Todos";
            IdFiltro = string.Empty;
            IniciativaFiltro = string.Empty;
            await AplicarFiltros();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }

    /// <summary>
    /// Implementación personalizada de RelayCommand para comandos asíncronos.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

        public async void Execute(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try
            {
                await _executeAsync();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
