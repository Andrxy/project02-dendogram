using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Project02_Dendogram.Presentation
{
    public partial class ClusteringJerarquico : Window
    {
        private readonly ClusteringModel _model;
        private readonly ClusteringController _controller;

        public ClusteringJerarquico()
        {
            InitializeComponent();
            _model = new ClusteringModel();
            _controller = new ClusteringController(_model);

            // Suscribir eventos
            Metricas.SelectionChanged += Metricas_SelectionChanged;

            UpdateUI();
        }

        #region Event Handlers

        private void CargarCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources");
                resourcesPath = Path.GetFullPath(resourcesPath);

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|TSV files (*.tsv)|*.tsv|All files (*.*)|*.*",
                    InitialDirectory = Directory.Exists(resourcesPath)
                        ? resourcesPath
                        : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _controller.LoadFile(openFileDialog.FileName);

                    // Actualizar UI
                    NombreArchivo.Text = $"✓ {Path.GetFileName(_model.LoadedFilePath)}";
                    NombreArchivo.Foreground = System.Windows.Media.Brushes.Green;
                    NombreArchivo.FontStyle = FontStyles.Normal;
                    CargarJSON.IsEnabled = true;

                    UpdateStatusText(_model.StatusMessage);
                    UpdateDatasetInfo();

                    MessageBox.Show(
                        $"Archivo cargado correctamente:\n\n{Path.GetFileName(_model.LoadedFilePath)}",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar el archivo:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                UpdateStatusText("Error al cargar archivo");
            }
        }

        private void Metricas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_controller == null) return;

            if (Metricas.SelectedItem is ComboBoxItem selectedItem)
            {
                string metricTag = selectedItem.Tag?.ToString() ?? "euclidean";
                _controller.ChangeDistanceMetric(metricTag);
                UpdateStatusText(_model.StatusMessage);
            }
        }

        private void CargarJSON_Click(object sender, RoutedEventArgs e)
        {
            if (!_model.IsFileLoaded)
            {
                MessageBox.Show(
                    "Por favor, cargue un archivo CSV primero.",
                    "Advertencia",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Confirmar ejecución
            var result = MessageBox.Show(
                "¿Está seguro de ejecutar el clustering con la configuración actual?\n\n" +
                "Este proceso puede tardar varios minutos dependiendo del tamaño del dataset.",
                "Confirmar Ejecución",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Deshabilitar UI durante procesamiento
                CargarJSON.IsEnabled = false;
                CargarJSON.Content = "⏳ Procesando...";
                UpdateStatusText("Iniciando clustering...");

                // Capturar configuración
                CaptureVariableConfiguration();
                CaptureNormalizationConfiguration();

                // Ejecutar clustering
                _controller.ExecuteClustering();

                // Mostrar éxito
                MessageBox.Show(
                    "¡Clusterización completada exitosamente!\n\n" +
                    "El dendrograma ha sido generado y exportado a JSON.",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                UpdateStatusText(_model.StatusMessage);
                UpdateDatasetInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante la clusterización:\n\n{ex.Message}\n\n" +
                    "Revise la configuración e intente nuevamente.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                UpdateStatusText("Error en clusterización");
            }
            finally
            {
                CargarJSON.IsEnabled = true;
                CargarJSON.Content = "🎬 Ejecutar Clusterización";
            }
        }

        #endregion

        #region Configuration Capture

        private void CaptureVariableConfiguration()
        {
            // Variables numéricas
            UpdateVariable("budget", chkBudget, txtBudget);
            UpdateVariable("popularity", chkPopularity, txtPopularity);
            UpdateVariable("revenue", chkRevenue, txtRevenue);
            UpdateVariable("runtime", chkRuntime, txtRuntime);
            UpdateVariable("voteaverage", chkVoteAverage, txtVoteAverage);
            UpdateVariable("votecount", chkVoteCount, txtVoteCount);
            UpdateVariable("releaseyear", chkReleaseYear, txtReleaseYear);

            // Variables categóricas
            UpdateVariable("genres", chkGenres, txtGenres);
            UpdateVariable("cast", chkCast, txtCast);
            UpdateVariable("directors", chkDirector, txtDirector);
            UpdateVariable("keywords", chkKeywords, txtKeywords);
            UpdateVariable("companies", chkCompanies, txtCompanies);
            UpdateVariable("languages", chkLanguages, txtLanguages);
        }

        private void UpdateVariable(string name, CheckBox checkbox, TextBox textbox)
        {
            bool isEnabled = checkbox.IsChecked == true;
            double weight = ParseWeight(textbox.Text);
            _controller.UpdateVariableConfig(name, isEnabled, weight);
        }

        private void CaptureNormalizationConfiguration()
        {
            UpdateNormalization("budget", cmbBudget);
            UpdateNormalization("popularity", cmbPopularity);
            UpdateNormalization("revenue", cmbRevenue);
            UpdateNormalization("runtime", cmbRuntime);
            UpdateNormalization("voteaverage", cmbVoteAverage);
            UpdateNormalization("votecount", cmbVoteCount);
            UpdateNormalization("releaseyear", cmbReleaseYear);
        }

        private void UpdateNormalization(string featureName, ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item)
            {
                string normalizationType = item.Tag?.ToString() ?? "minmax";
                _controller.UpdateNormalizationStrategy(featureName, normalizationType);
            }
        }

        private double ParseWeight(string text)
        {
            if (double.TryParse(text.Replace('.', ','), out double weight))
            {
                return Math.Max(0, weight); // Evitar pesos negativos
            }
            return 1.0;
        }

        #endregion

        #region UI Updates

        private void UpdateStatusText(string message)
        {
            StatusText.Text = message;
        }

        private void UpdateDatasetInfo()
        {
            if (_model.IsFileLoaded && _model.Movies != null)
            {
                DatasetInfo.Text = _model.GetDatasetInfo();
            }
            else
            {
                DatasetInfo.Text = "Cargue un archivo para ver información";
            }
        }

        private void UpdateUI()
        {
            UpdateStatusText(_model.StatusMessage);
            CargarJSON.IsEnabled = _model.IsFileLoaded;
            UpdateDatasetInfo();
        }

        #endregion
    }
}