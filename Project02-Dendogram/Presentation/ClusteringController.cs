using System;
using System.IO;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Services;

namespace Project02_Dendogram.Presentation
{
    /// <summary>
    /// Controlador que maneja la lógica de negocio y coordina entre el Modelo y la Vista
    /// </summary>
    internal class ClusteringController
    {
        private readonly ClusteringModel _model;

        public ClusteringController(ClusteringModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Carga un archivo TSV
        /// </summary>
        public void LoadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("La ruta del archivo no puede estar vacía");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"El archivo no existe: {filePath}");

            _model.LoadedFilePath = filePath;
            _model.StatusMessage = $"Archivo cargado: {Path.GetFileName(filePath)}";
        }

        /// <summary>
        /// Cambia la métrica de distancia
        /// </summary>
        public void ChangeDistanceMetric(string metricName)
        {
            _model.ConfigurationManager.SetDistanceMetric(metricName);
            _model.StatusMessage = $"Métrica de distancia: {metricName}";
        }

        /// <summary>
        /// Actualiza la estrategia de normalización para una variable específica
        /// </summary>
        public void UpdateNormalizationStrategy(string featureName, string strategyName)
        {
            _model.ConfigurationManager.SetNormalizationStrategyByName(featureName, strategyName);
            Console.WriteLine($"✓ Normalización de {featureName}: {strategyName}");
        }

        /// <summary>
        /// Ejecuta todo el proceso de clusterización
        /// </summary>
        public void ExecuteClustering()
        {
            if (!_model.IsFileLoaded)
                throw new InvalidOperationException("No se ha cargado ningún archivo");

            // 1. Parsear archivo TSV
            _model.StatusMessage = "Procesando datos...";
            CSVParser parser = new CSVParser(_model.LoadedFilePath);
            _model.Movies = parser.ParseMovies();
            _model.StatusMessage = $"Películas cargadas: {_model.Movies.Count}";

            // 2. Vectorizar películas
            _model.StatusMessage = "Vectorizando películas...";
            _model.VectorizationService = new VectorizationService();
            _model.VectorizationService.VectorizeMovies(_model.Movies);
            _model.StatusMessage = "Vectorización completada";

            // 3. Configurar normalización
            _model.ConfigurationManager.InitializeWeightsForDataset(_model.VectorizationService);

            // 4. Aplicar configuración de variables
            ApplyVariableConfiguration();

            // 5. Normalizar y ponderar
            _model.StatusMessage = "Normalizando datos...";
            _model.ConfigurationManager.NormalizationConfig.Normalize(_model.Movies);
            WeightApplier.ApplyWeights(_model.Movies);
            _model.StatusMessage = "Normalización y ponderación aplicadas";

            // 6. Construir dendrograma
            _model.StatusMessage = "Construyendo dendrograma...";
            DendogramBuilder builder = new DendogramBuilder(_model.ConfigurationManager.DistanceStrategy);
            _model.DendrogramRoot = builder.BuildDendogram(_model.Movies);
            _model.StatusMessage = "Dendrograma construido exitosamente";

            // 7. Imprimir dendrograma en consola
            PrintDendrogram(_model.DendrogramRoot, "");

            JSONExporter exporter = new JSONExporter();
            exporter.ExportToFile(_model.DendrogramRoot);

            string dendrogramText = GetDendrogramString(_model.DendrogramRoot);
            System.Windows.MessageBox.Show(
                dendrogramText,
                "Dendrograma Generado",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Aplica la configuración de pesos de las variables
        /// </summary>
        private void ApplyVariableConfiguration()
        {
            var config = _model.VariableConfig;
            var weights = new double[7];

            // Variables numéricas
            weights[0] = config.Budget.IsEnabled ? config.Budget.Weight : 0;
            weights[1] = config.Popularity.IsEnabled ? config.Popularity.Weight : 0;
            weights[2] = config.Revenue.IsEnabled ? config.Revenue.Weight : 0;
            weights[3] = config.Runtime.IsEnabled ? config.Runtime.Weight : 0;
            weights[4] = config.VoteAverage.IsEnabled ? config.VoteAverage.Weight : 0;
            weights[5] = config.VoteCount.IsEnabled ? config.VoteCount.Weight : 0;
            weights[6] = config.ReleaseYear.IsEnabled ? config.ReleaseYear.Weight : 0;

            // Aplicar pesos numéricos
            for (int i = 0; i < 7; i++)
            {
                _model.ConfigurationManager.UpdateWeight(i, weights[i]);
            }

            int currentIndex = 7;

            // Variables categóricas
            currentIndex = ApplyCategoricalWeight(
                config.Genres,
                _model.VectorizationService.Indexer.GenreIndex.Count,
                currentIndex
            );

            currentIndex = ApplyCategoricalWeight(
                config.Cast,
                _model.VectorizationService.Indexer.CastIndex.Count,
                currentIndex
            );

            currentIndex = ApplyCategoricalWeight(
                config.Director,
                _model.VectorizationService.Indexer.DirectorIndex.Count,
                currentIndex
            );

            currentIndex = ApplyCategoricalWeight(
                config.Keywords,
                _model.VectorizationService.Indexer.KeywordsIndex.Count,
                currentIndex
            );

            currentIndex = ApplyCategoricalWeight(
                config.ProductionCountries,
                _model.VectorizationService.Indexer.ProductionCountriesIndex.Count,
                currentIndex
            );

            currentIndex = ApplyCategoricalWeight(
                config.SpokenLanguages,
                _model.VectorizationService.Indexer.SpokenLanguagesIndex.Count,
                currentIndex
            );

            Console.WriteLine($"\n⚙️ Configuración de pesos aplicada. Total features: {currentIndex}");
        }

        /// <summary>
        /// Aplica pesos a una variable categórica
        /// </summary>
        private int ApplyCategoricalWeight(VariableSettings settings, int count, int startIndex)
        {
            if (settings.IsEnabled)
            {
                for (int i = 0; i < count; i++)
                {
                    _model.ConfigurationManager.UpdateWeight(startIndex + i, settings.Weight);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    _model.ConfigurationManager.UpdateWeight(startIndex + i, 0);
                }
            }

            return startIndex + count;
        }

        /// <summary>
        /// Actualiza la configuración de una variable
        /// </summary>
        public void UpdateVariableConfig(string variableName, bool isEnabled, double weight)
        {
            var config = _model.VariableConfig;

            switch (variableName.ToLower())
            {
                case "budget":
                    config.Budget.IsEnabled = isEnabled;
                    config.Budget.Weight = weight;
                    break;
                case "popularity":
                    config.Popularity.IsEnabled = isEnabled;
                    config.Popularity.Weight = weight;
                    break;
                case "revenue":
                    config.Revenue.IsEnabled = isEnabled;
                    config.Revenue.Weight = weight;
                    break;
                case "runtime":
                    config.Runtime.IsEnabled = isEnabled;
                    config.Runtime.Weight = weight;
                    break;
                case "voteaverage":
                    config.VoteAverage.IsEnabled = isEnabled;
                    config.VoteAverage.Weight = weight;
                    break;
                case "votecount":
                    config.VoteCount.IsEnabled = isEnabled;
                    config.VoteCount.Weight = weight;
                    break;
                case "releaseyear":
                    config.ReleaseYear.IsEnabled = isEnabled;
                    config.ReleaseYear.Weight = weight;
                    break;
                case "genres":
                    config.Genres.IsEnabled = isEnabled;
                    config.Genres.Weight = weight;
                    break;
                case "cast":
                    config.Cast.IsEnabled = isEnabled;
                    config.Cast.Weight = weight;
                    break;
                case "director":
                    config.Director.IsEnabled = isEnabled;
                    config.Director.Weight = weight;
                    break;
                case "keywords":
                    config.Keywords.IsEnabled = isEnabled;
                    config.Keywords.Weight = weight;
                    break;
                case "countries":
                    config.ProductionCountries.IsEnabled = isEnabled;
                    config.ProductionCountries.Weight = weight;
                    break;
                case "languages":
                    config.SpokenLanguages.IsEnabled = isEnabled;
                    config.SpokenLanguages.Weight = weight;
                    break;
            }
        }

        /// <summary>
        /// Obtiene una lista de películas para mostrar
        /// </summary>
        public System.Collections.Generic.List<Movie> GetMoviesForDisplay()
        {
            if (_model.Movies == null)
                return new System.Collections.Generic.List<Movie>();

            var moviesList = new System.Collections.Generic.List<Movie>();
            var iterator = _model.Movies.CreateIterator();
            while (iterator.HasNext())
            {
                moviesList.Add(iterator.Next());
            }
            return moviesList;
        }

        /// <summary>
        /// Imprime el dendrograma en consola
        /// </summary>
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

        private string GetDendrogramString(Cluster node, string indent = "")
        {
            if (node == null)
                return string.Empty;

            string result = "";

            if (node.IsLeaf)
            {
                result += $"{indent}- {node.Movie.Title}\n";
            }
            else
            {
                result += $"{indent}+ Merge (distancia: {node.Distance:0.00})\n";
                result += GetDendrogramString(node.Left, indent + "  ");
                result += GetDendrogramString(node.Right, indent + "  ");
            }

            return result;
        }

    }
}