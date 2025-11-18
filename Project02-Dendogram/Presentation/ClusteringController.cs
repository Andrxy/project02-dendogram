using System;
using System.IO;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Services;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Project02_Dendogram.Presentation
{
    internal class ClusteringController
    {
        private readonly ClusteringModel _model;
        private readonly ConfigurationManager _config;

        public ClusteringController(ClusteringModel model)
        {
            _model = model;
            _config = ConfigurationManager.GetInstance();
        }

        public void LoadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("La ruta del archivo no puede estar vacía");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"El archivo no existe: {filePath}");

            _model.LoadedFilePath = filePath;
            _model.StatusMessage = $"Archivo cargado: {Path.GetFileName(filePath)}";
        }

        public void ChangeDistanceMetric(string metricName)
        {
            _config.SetDistanceMetric(metricName);
            _model.StatusMessage = $"Métrica de distancia: {metricName}";
        }

        public void UpdateNormalizationStrategy(string featureName, string strategyName)
        {
            _config.SetNormalizationStrategy(featureName, strategyName);
            _model.StatusMessage = $"Normalización de '{featureName}' cambiada a '{strategyName}'";
        }

        public void UpdateVariableConfig(string variableName, bool isEnabled, double weight)
        {
            _config.UpdateVariableConfig(variableName, isEnabled, weight);
            _model.StatusMessage = $"Variable '{variableName}' actualizada: Enabled={isEnabled}, Weight={weight}";
        }

        public void ExecuteClustering()
        {
            if (!_model.IsFileLoaded)
                throw new InvalidOperationException("No se ha cargado ningún archivo");

            try
            {
                // 1. Parsear archivo
                _model.StatusMessage = "Paso 1/6: Cargando datos...";
                ParseMoviesFromFile();

                // 2. Vectorizar películas
                _model.StatusMessage = "Paso 2/6: Vectorizando características...";
                VectorizeMovies();

                // 3. Normalizar datos
                _model.StatusMessage = "Paso 3/6: Normalizando datos...";
                _config.NormalizeDataset(_model.Movies);

                // 4. Aplicar pesos
                _model.StatusMessage = "Paso 4/6: Aplicando ponderación...";
                _config.ApplyWeightsToDataset(_model.Movies, _model.VectorizationService);

                // 5. Construir dendrograma
                _model.StatusMessage = "Paso 5/6: Construyendo dendrograma...";
                BuildDendogram();

                // 6. Exportar a JSON
                ExportDendogramToJSON();

                _model.StatusMessage = "✓ Clustering completado exitosamente";
            }
            catch (Exception ex)
            {
                _model.StatusMessage = $"✗ Error durante el clustering: {ex.Message}";
                throw;
            }
        }

        private void ParseMoviesFromFile()
        {
            CSVParser parser = new CSVParser(_model.LoadedFilePath);
            _model.Movies = parser.ParseMovies();

            if (_model.Movies.Count == 0)
                throw new InvalidOperationException("No se pudieron cargar películas del archivo");

            _model.StatusMessage = $"✓ {_model.Movies.Count} películas cargadas";
        }

        private void VectorizeMovies()
        {
            _model.VectorizationService = new VectorizationService();
            _model.VectorizationService.VectorizeMovies(_model.Movies);
            _model.StatusMessage = "✓ Vectorización completada";
        }

        private void BuildDendogram()
        {
            DendogramBuilder builder = new DendogramBuilder(_config.DistanceStrategy);
            _model.DendrogramRoot = builder.BuildDendogram(_model.Movies);
            _model.StatusMessage = "✓ Dendrograma construido";
        }

        private void ExportDendogramToJSON()
        {
            JSONExporter exporter = new JSONExporter();
            exporter.ExportToFile(_model.DendrogramRoot);
            _model.StatusMessage = "Dendrograma exportado a JSON";
        }
    }
}
