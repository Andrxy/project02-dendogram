using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Services;

namespace Project02_Dendogram.Presentation
{
    /// <summary>
    /// Modelo de datos para la vista de Clustering
    /// Contiene el estado actual del sistema sin lógica de configuración
    /// </summary>
    internal class ClusteringModel
    {
        public CustomList<Movie> Movies { get; set; }
        public Cluster DendrogramRoot { get; set; }
        public string LoadedFilePath { get; set; }
        public VectorizationService VectorizationService { get; set; }
        public string StatusMessage { get; set; }

        public bool IsFileLoaded => !string.IsNullOrEmpty(LoadedFilePath);
        public bool IsProcessingComplete => DendrogramRoot != null;
        public bool IsReadyForClustering => Movies != null && Movies.Count > 0;

        public ClusteringModel()
        {
            StatusMessage = "Sistema listo. Cargue un archivo para comenzar.";
        }

        public void Reset()
        {
            Movies = null;
            DendrogramRoot = null;
            LoadedFilePath = null;
            VectorizationService = null;
            StatusMessage = "Sistema reiniciado. Cargue un archivo para comenzar.";
        }

        public string GetDatasetInfo()
        {
            if (Movies == null || Movies.Count == 0)
                return "No hay datos cargados";

            return $"Películas cargadas: {Movies.Count}";
        }
    }
}
