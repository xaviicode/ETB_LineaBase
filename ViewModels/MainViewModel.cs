using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LineaBaseETB_V2.Models;
using LineaBaseETB_V2.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LineaBaseETB_V2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Filtros para Work Items
        public ObservableCollection<string> EstadosDisponibles { get; } = new ObservableCollection<string>
        {
            "En revisión Líder Técnico", "En Revisión", "Pendiente Autorización QA", "Rechazado", "En Branch QA", "Desplegado QA",
            "Autorizado Release", "En Branch Producción", "Desplegado QA-UNO", "Desplegado Release", "Done", "Pendiente despliegue en Urgentes",
            "En Branch QA-UNO",
        };

        private string _estadoSeleccionado;
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    // Si quieres filtrado automático al cambiar, puedes llamar aquí a AplicarFiltros()
                }
            }
        }

        private string _idFiltro;
        public string IdFiltro
        {
            get => _idFiltro;
            set
            {
                if (SetProperty(ref _idFiltro, value))
                {
                    // Si quieres filtrado automático al escribir, puedes llamar aquí a AplicarFiltros()
                }
            }
        }

        private string _iniciativaFiltro;
        public string IniciativaFiltro
        {
            get => _iniciativaFiltro;
            set
            {
                if (SetProperty(ref _iniciativaFiltro, value))
                {
                    // Si quieres filtrado automático al escribir, puedes llamar aquí a AplicarFiltros()
                }
            }
        }

        // Comando de filtro
        public ICommand FiltrarCommand { get; }

        // --- Código original ---
        private string _organization;
        private string _pat;
        private string _proyectoSeleccionado;
        private bool _isLoading;
        private string _statusMessage;

        private AzureDevOpsService _azureService;

        public ObservableCollection<string> Proyectos { get; } = new ObservableCollection<string>();
        public ObservableCollection<WorkItemModel> WorkItems { get; } = new ObservableCollection<WorkItemModel>();

        private ICollectionView _workItemsView;
        public ICollectionView WorkItemsView
        {
            get => _workItemsView;
            private set => SetProperty(ref _workItemsView, value);
        }

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

        private Brush _proyectoBorderBrush = Brushes.Gray;
        public Brush ProyectoBorderBrush
        {
            get => _proyectoBorderBrush;
            set => SetProperty(ref _proyectoBorderBrush, value);
        }

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

        public string ProyectoSeleccionado
        {
            get => _proyectoSeleccionado;
            set
            {
                if (SetProperty(ref _proyectoSeleccionado, value))
                {
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

        public bool CanConsultar =>
            !IsLoading &&
            !string.IsNullOrWhiteSpace(Organization) &&
            !string.IsNullOrWhiteSpace(Pat) &&
            !string.IsNullOrWhiteSpace(ProyectoSeleccionado);

        public ICommand ConsultarCommand { get; }

        public MainViewModel()
        {
            ConsultarCommand = new RelayCommand(async () => await ConsultarWorkItemsAsync(), () => CanConsultar);

            // Inicializa el comando de filtro
            FiltrarCommand = new RelayCommand(async () => await AplicarFiltros());

            WorkItemsView = CollectionViewSource.GetDefaultView(WorkItems);
            WorkItemsView.Filter = null; // Sin filtro por defecto
        }

        private void ClearData()
        {
            Proyectos.Clear();
            WorkItems.Clear();
            ProyectoSeleccionado = null;
            StatusMessage = string.Empty;
        }

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

        private async Task ConsultarWorkItemsAsync()
        {
            if (_azureService == null)
            {
                StatusMessage = "Primero debe cargar los proyectos.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ProyectoSeleccionado))
            {
                StatusMessage = "Seleccione un proyecto.";
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Consultando Work Items...";

                WorkItems.Clear();

                var items = await _azureService.ObtenerWorkItemsAsync(ProyectoSeleccionado);

                foreach (var wi in items)
                    WorkItems.Add(wi);

                StatusMessage = $"Se obtuvieron {WorkItems.Count} Work Items.";
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

        private void ValidarCampos()
        {
            OrganizationBorderBrush = string.IsNullOrWhiteSpace(Organization) ? Brushes.Red : Brushes.Gray;
            PatBorderBrush = string.IsNullOrWhiteSpace(Pat) ? Brushes.Red : Brushes.Gray;
            ProyectoBorderBrush = string.IsNullOrWhiteSpace(ProyectoSeleccionado) ? Brushes.Red : Brushes.Gray;
        }

        // Lógica del comando de filtro
        private async Task AplicarFiltros()
        {
            WorkItemsView.Filter = item =>
            {
                var workItem = item as WorkItemModel;
                if (workItem == null)
                    return false;

                // Filtrado por Estado
                bool estadoOk = string.IsNullOrWhiteSpace(EstadoSeleccionado) ||
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

    // Tu implementación custom de RelayCommand se mantiene igual
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
