
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Services;
using Project02_Dendogram.Strategies.Distance;

namespace Project02_Dendogram.Presentation
{
    public partial class ClusteringJerarquico : Window
    {
        private CustomList<Movie> _movies;
        private VectorizationService _vectorization;
        private string _loadedFilePath;
        private ConfigurationManager _config;

        public ClusteringJerarquico()
        {
            _config = ConfigurationManager.Instance;
            InitializeComponent();
            Metricas.SelectionChanged += Metricas_SelectionChanged;
            StatusText.Text = "Esperando carga de archivo...";
        }
        private void CargarCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "TSV files (*.tsv)|*.tsv|All files (*.*)|*.*",
                    InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/movies.tsv")
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _loadedFilePath = openFileDialog.FileName;
                    NombreArchivo.Content = Path.GetFileName(_loadedFilePath);
                    NombreArchivo.Foreground = System.Windows.Media.Brushes.Green;

                    StatusText.Text = "Archivo cargado. Configure variables y ejecute clusterización.";
                    CargarJSON.IsEnabled = true;

                    MessageBox.Show($"Archivo cargado correctamente:\n{Path.GetFileName(_loadedFilePath)}",
                                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Error al cargar archivo";
            }
        }

        private void Metricas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_config == null || StatusText == null) return;
            if (Metricas.SelectedItem is ComboBoxItem selectedItem)
            {
                string metricTag = selectedItem.Tag?.ToString() ?? "euclidean";
                _config.SetDistanceMetric(metricTag);
                StatusText.Text = $"Métrica de distancia: {selectedItem.Content}";
            }
        }

        private void CargarJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Procesando datos...";
                CargarJSON.IsEnabled = false;

                // 1. Parsear archivo TSV
                CSVParser parser = new CSVParser(_loadedFilePath);
                _movies = parser.ParseMovies();
                StatusText.Text = $"Películas cargadas: {_movies.Count}";

                // 2. Vectorizar películas
                _vectorization = new VectorizationService();
                _vectorization.VectorizeMovies(_movies);
                StatusText.Text = "Vectorización completada";

                // 3. Configurar normalización (usando valores por defecto o personalizados)
                _config.InitializeWeightsForDataset(_vectorization);

                // 4. Aplicar configuración de variables (pesos y checkboxes)
                ApplyVariableConfiguration();

                // 5. Normalizar y ponderar
                _config.NormalizationConfig.Normalize(_movies);
                WeightApplier.ApplyWeights(_movies);
                StatusText.Text = "Normalización y ponderación aplicadas";

                // 6. Construir dendrograma
                DendogramBuilder builder = new DendogramBuilder(_config.DistanceStrategy);
                Cluster root = builder.BuildDendogram(_movies);
                StatusText.Text = "Dendrograma construido exitosamente";

                // 7. Mostrar resultados en la tabla
                DisplayResults();

                // 8. Mostrar dendrograma textual en consola (opcional)
                Console.WriteLine("\n🌳 Dendrograma textual:");
                PrintDendrogram(root, "");

                MessageBox.Show("Clusterización completada exitosamente!",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                CargarJSON.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la clusterización:\n{ex.Message}\n\n{ex.StackTrace}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Error en clusterización";
                CargarJSON.IsEnabled = true;
            }
        }

        private void ApplyVariableConfiguration()
        {
            // Obtener pesos de las variables numéricas
            double[] numericWeights = new double[7];
            numericWeights[0] = chkBudget.IsChecked == true ? ParseWeight(txtBudget.Text) : 0;
            numericWeights[1] = chkPopularity.IsChecked == true ? ParseWeight(txtPopularity.Text) : 0;
            numericWeights[2] = chkRevenue.IsChecked == true ? ParseWeight(txtRevenue.Text) : 0;
            numericWeights[3] = chkRuntime.IsChecked == true ? ParseWeight(txtRuntime.Text) : 0;
            numericWeights[4] = chkVoteAverage.IsChecked == true ? ParseWeight(txtVoteAverage.Text) : 0;
            numericWeights[5] = chkVoteCount.IsChecked == true ? ParseWeight(txtVoteCount.Text) : 0;
            numericWeights[6] = chkReleaseYear.IsChecked == true ? ParseWeight(txtReleaseYear.Text) : 0;

            // Aplicar pesos numéricos
            for (int i = 0; i < 7; i++)
            {
                _config.UpdateWeight(i, numericWeights[i]);
            }

            // Obtener pesos de las variables categóricas
            int currentIndex = 7;

            // Géneros
            if (chkGenres.IsChecked == true)
            {
                double genreWeight = ParseWeight(txtGenres.Text);
                for (int i = 0; i < _vectorization.Indexer.GenreIndex.Count; i++)
                {
                    _config.UpdateWeight(currentIndex++, genreWeight);
                }
            }
            else
            {
                currentIndex += _vectorization.Indexer.GenreIndex.Count;
            }

            // Cast
            if (chkCast.IsChecked == true)
            {
                double castWeight = ParseWeight(txtCast.Text);
                for (int i = 0; i < _vectorization.Indexer.CastIndex.Count; i++)
                {
                    _config.UpdateWeight(currentIndex++, castWeight);
                }
            }
            else
            {
                currentIndex += _vectorization.Indexer.CastIndex.Count;
            }

            // Director
            if (chkDirector.IsChecked == true)
            {
                double directorWeight = ParseWeight(txtDirector.Text);
                for (int i = 0; i < _vectorization.Indexer.DirectorIndex.Count; i++)
                {
                    _config.UpdateWeight(currentIndex++, directorWeight);
                }
            }
            else
            {
                currentIndex += _vectorization.Indexer.DirectorIndex.Count;
            }

            // Keywords
            if (chkKeywords.IsChecked == true)
            {
                double keywordWeight = ParseWeight(txtKeywords.Text);
                for (int i = 0; i < _vectorization.Indexer.KeywordsIndex.Count; i++)
                {
                    _config.UpdateWeight(currentIndex++, keywordWeight);
                }
            }
            else
            {
                currentIndex += _vectorization.Indexer.KeywordsIndex.Count;
            }

            // Production Countries
            if (chkCountries.IsChecked == true)
            {
                double countryWeight = ParseWeight(txtCountries.Text);
                for (int i = 0; i < _vectorization.Indexer.ProductionCountriesIndex.Count; i++)
                {
                    _config.UpdateWeight(currentIndex++, countryWeight);
                }
            }
            else
            {
                currentIndex += _vectorization.Indexer.ProductionCountriesIndex.Count;
            }

            // Spoken Languages
            if (chkLanguages.IsChecked == true)
            {
                double langWeight = ParseWeight(txtLanguages.Text);
                for (int i = 0; i < _vectorization.Indexer.SpokenLanguagesIndex.Count; i++)
                {
                    _config.UpdateWeight(currentIndex++, langWeight);
                }
            }

            Console.WriteLine($"\n⚙️ Configuración de pesos aplicada. Total features: {currentIndex}");
        }

        private double ParseWeight(string text)
        {
            if (double.TryParse(text.Replace('.', ','), out double weight))
            {
                return weight;
            }
            return 1.0; // Valor por defecto
        }

        private void DisplayResults()
        {
            var moviesList = new System.Collections.Generic.List<Movie>();
            var iterator = _movies.CreateIterator();
            while (iterator.HasNext())
            {
                moviesList.Add(iterator.Next());
            }
            Tabla1.ItemsSource = moviesList;
            StatusText.Text = $"Mostrando {moviesList.Count} películas";
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabla1.SelectedItem is Movie selectedMovie)
            {
                StatusText.Text = $"Seleccionado: {selectedMovie.Title} ({selectedMovie.ReleaseYear})";
            }
        }

        private void PrintDendrogram(Cluster node, string indent)
        {
            if (node == null) return;

            if (node.IsLeaf)
            {
                Console.WriteLine($"{indent}- {node.Movie.Title}");
            }
            else
            {
                Console.WriteLine($"{indent}+ Merge (distancia: {node.Distance:0.00})");
                PrintDendrogram(node.Left, indent + "  ");
                PrintDendrogram(node.Right, indent + "  ");
            }
        }
    }
}