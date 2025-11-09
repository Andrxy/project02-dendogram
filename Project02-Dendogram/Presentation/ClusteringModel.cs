using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Services;

namespace Project02_Dendogram.Presentation
{
   //Aqui estan declaradas las variables que usa el View
    internal class ClusteringModel
    {
        // Datos principales
        public CustomList<Movie> Movies { get; set; }
        public Cluster DendrogramRoot { get; set; }
        public string LoadedFilePath { get; set; }

        
        public VectorizationService VectorizationService { get; set; }
        public ConfigurationManager ConfigurationManager { get; private set; }

        // Configuración de variables
        public VariableConfiguration VariableConfig { get; set; }

        // Estado de la aplicación
        public string StatusMessage { get; set; }
        public bool IsFileLoaded => !string.IsNullOrEmpty(LoadedFilePath);
        public bool IsProcessingComplete => DendrogramRoot != null;

        public ClusteringModel()
        {
            ConfigurationManager = ConfigurationManager.Instance;
            VariableConfig = new VariableConfiguration();
            StatusMessage = "Esperando carga de archivo...";
        }

        
        // Reinicia el modelo a su estado inicial
        
        public void Reset()
        {
            Movies = null;
            DendrogramRoot = null;
            LoadedFilePath = null;
            VectorizationService = null;
            StatusMessage = "Esperando carga de archivo...";
        }

        public void ResetWeigthedVector()
        {
            IIterator<Movie> it = Movies.CreateIterator();
            while (it.HasNext())
            {
                Movie movie = it.Next();
                movie.WeightedFeatureVector = movie.FeatureVector;
            }
        }
    }

   
    // Configuración de variables numéricas y categóricas
    
    internal class VariableConfiguration
    {
        // Variables numéricas
        public VariableSettings Budget { get; set; }
        public VariableSettings Popularity { get; set; }
        public VariableSettings Revenue { get; set; }
        public VariableSettings Runtime { get; set; }
        public VariableSettings VoteAverage { get; set; }
        public VariableSettings VoteCount { get; set; }
        public VariableSettings ReleaseYear { get; set; }

        // Variables categóricas
        public VariableSettings Genres { get; set; }
        public VariableSettings Cast { get; set; }
        public VariableSettings Director { get; set; }
        public VariableSettings Keywords { get; set; }
        public VariableSettings ProductionCompanies { get; set; }
        public VariableSettings SpokenLanguages { get; set; }

        public VariableConfiguration()
        {
            // Inicializar variables
            Budget = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            Popularity = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            Revenue = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            Runtime = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            VoteAverage = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            VoteCount = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            ReleaseYear = new VariableSettings { IsEnabled = true, Weight = 1.0 };

          
            Genres = new VariableSettings { IsEnabled = true, Weight = 15.0 };
            Cast = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            Director = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            Keywords = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            ProductionCompanies = new VariableSettings { IsEnabled = true, Weight = 1.0 };
            SpokenLanguages = new VariableSettings { IsEnabled = true, Weight = 1.0 };
        }
    }

    
    // Configuración de cada variable
    
    internal class VariableSettings
    {
        public bool IsEnabled { get; set; }
        public double Weight { get; set; }
    }
}