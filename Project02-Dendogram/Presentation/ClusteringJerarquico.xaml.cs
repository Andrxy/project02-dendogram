using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Project02_Dendogram.Models;

namespace Project02_Dendogram.Presentation
{
    /// <summary>
    /// Vista para el clustering jerárquico - Patrón MVC
    /// </summary>
    public partial class ClusteringJerarquico : Window
    {
        // Model y Controller
        private readonly ClusteringModel _model;
        private readonly ClusteringController _controller;

        public ClusteringJerarquico()
        {
            InitializeComponent();
            _model = new ClusteringModel();
            _controller = new ClusteringController(_model);

            // Suscribir después de crear el controller
            Metricas.SelectionChanged += Metricas_SelectionChanged;

            UpdateUI();
        }

        #region Event Handlers

        /// <summary>
        /// Maneja el clic en el botón de cargar archivo
        /// </summary>
        private void CargarCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources");
                resourcesPath = Path.GetFullPath(resourcesPath);

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "TSV files (*.tsv)|*.tsv|All files (*.*)|*.*",
                    InitialDirectory = Directory.Exists(resourcesPath)
                        ? resourcesPath
                        : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Delegar al controller
                    _controller.LoadFile(openFileDialog.FileName);

                    // Actualizar UI
                    NombreArchivo.Content = Path.GetFileName(_model.LoadedFilePath);
                    NombreArchivo.Foreground = System.Windows.Media.Brushes.Green;
                    CargarJSON.IsEnabled = true;

                    UpdateStatusText(_model.StatusMessage);

                    MessageBox.Show($"Archivo cargado correctamente:\n{Path.GetFileName(_model.LoadedFilePath)}",
                                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusText("Error al cargar archivo");
            }
        }

        /// <summary>
        /// Maneja el cambio de métrica de distancia
        /// </summary>
        private void Metricas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_controller == null) return; // evita NRE durante inicialización
            if (Metricas.SelectedItem is ComboBoxItem selectedItem)
            {
                string metricTag = selectedItem.Tag?.ToString() ?? "euclidean";
                _controller.ChangeDistanceMetric(metricTag);
                UpdateStatusText(_model.StatusMessage);
            }
        }

        /// <summary>
        /// Maneja el clic en el botón de ejecutar clusterización
        /// </summary>
        private void CargarJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CargarJSON.IsEnabled = false;
                UpdateStatusText("Procesando datos...");

                // Capturar configuración de la UI
                CaptureVariableConfiguration();

                // Capturar configuración de normalización
                CaptureNormalizationConfiguration();

                // Ejecutar clusterización a través del controller
                _controller.ExecuteClustering();

                // Mostrar resultados
                DisplayResults();

                MessageBox.Show("Clusterización completada exitosamente!",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                UpdateStatusText(_model.StatusMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la clusterización:\n{ex.Message}\n\n{ex.StackTrace}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusText("Error en clusterización");
            }
            finally
            {
                CargarJSON.IsEnabled = true;
            }
        }

        /// <summary>
        /// Maneja la selección de una película en la tabla
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabla1.SelectedItem is Movie selectedMovie)
            {
                UpdateStatusText($"Seleccionado: {selectedMovie.Title} ({selectedMovie.ReleaseYear})");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Captura la configuración de variables desde la UI
        /// </summary>
        private void CaptureVariableConfiguration()
        {
            // Variables numéricas
            _controller.UpdateVariableConfig("budget",
                chkBudget.IsChecked == true,
                ParseWeight(txtBudget.Text));

            _controller.UpdateVariableConfig("popularity",
                chkPopularity.IsChecked == true,
                ParseWeight(txtPopularity.Text));

            _controller.UpdateVariableConfig("revenue",
                chkRevenue.IsChecked == true,
                ParseWeight(txtRevenue.Text));

            _controller.UpdateVariableConfig("runtime",
                chkRuntime.IsChecked == true,
                ParseWeight(txtRuntime.Text));

            _controller.UpdateVariableConfig("voteaverage",
                chkVoteAverage.IsChecked == true,
                ParseWeight(txtVoteAverage.Text));

            _controller.UpdateVariableConfig("votecount",
                chkVoteCount.IsChecked == true,
                ParseWeight(txtVoteCount.Text));

            _controller.UpdateVariableConfig("releaseyear",
                chkReleaseYear.IsChecked == true,
                ParseWeight(txtReleaseYear.Text));

            // Variables categóricas
            _controller.UpdateVariableConfig("genres",
                chkGenres.IsChecked == true,
                ParseWeight(txtGenres.Text));

            _controller.UpdateVariableConfig("cast",
                chkCast.IsChecked == true,
                ParseWeight(txtCast.Text));

            _controller.UpdateVariableConfig("director",
                chkDirector.IsChecked == true,
                ParseWeight(txtDirector.Text));

            _controller.UpdateVariableConfig("keywords",
                chkKeywords.IsChecked == true,
                ParseWeight(txtKeywords.Text));

            _controller.UpdateVariableConfig("countries",
                chkCountries.IsChecked == true,
                ParseWeight(txtCountries.Text));

            _controller.UpdateVariableConfig("languages",
                chkLanguages.IsChecked == true,
                ParseWeight(txtLanguages.Text));
        }

        /// <summary>
        /// Captura la configuración de normalización desde la UI
        /// </summary>
        private void CaptureNormalizationConfiguration()
        {
            // Budget
            if (cmbBudget.SelectedItem is ComboBoxItem budgetItem)
            {
                string normalizationType = budgetItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("Budget", normalizationType);
            }

            // Popularity
            if (cmbPopularity.SelectedItem is ComboBoxItem popularityItem)
            {
                string normalizationType = popularityItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("Popularity", normalizationType);
            }

            // Revenue
            if (cmbRevenue.SelectedItem is ComboBoxItem revenueItem)
            {
                string normalizationType = revenueItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("Revenue", normalizationType);
            }

            // Runtime
            if (cmbRuntime.SelectedItem is ComboBoxItem runtimeItem)
            {
                string normalizationType = runtimeItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("Runtime", normalizationType);
            }

            // VoteAverage
            if (cmbVoteAverage.SelectedItem is ComboBoxItem voteAvgItem)
            {
                string normalizationType = voteAvgItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("VoteAverage", normalizationType);
            }

            // VoteCount
            if (cmbVoteCount.SelectedItem is ComboBoxItem voteCountItem)
            {
                string normalizationType = voteCountItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("VoteCount", normalizationType);
            }

            // ReleaseYear
            if (cmbReleaseYear.SelectedItem is ComboBoxItem releaseYearItem)
            {
                string normalizationType = releaseYearItem.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy("ReleaseYear", normalizationType);
            }
        }

        /// <summary>
        /// Parsea un peso desde texto
        /// </summary>
        private double ParseWeight(string text)
        {
            if (double.TryParse(text.Replace('.', ','), out double weight))
            {
                return weight;
            }
            return 1.0; // Valor por defecto
        }

        /// <summary>
        /// Muestra los resultados en la tabla
        /// </summary>
        private void DisplayResults()
        {
            var moviesList = _controller.GetMoviesForDisplay();
            Tabla1.ItemsSource = moviesList;
            UpdateStatusText($"Mostrando {moviesList.Count} películas");
        }

        /// <summary>
        /// Actualiza el texto de estado
        /// </summary>
        private void UpdateStatusText(string message)
        {
            StatusText.Text = message;
        }

        /// <summary>
        /// Actualiza toda la UI basada en el estado del modelo
        /// </summary>
        private void UpdateUI()
        {
            UpdateStatusText(_model.StatusMessage);
            CargarJSON.IsEnabled = _model.IsFileLoaded;
        }

        #endregion
    }
}