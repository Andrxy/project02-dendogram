using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Strategies.Normalization;
using Project02_Dendogram.Strategies.Distance;
using Project02_Dendogram.Models;
using System;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Services
{
    // Singleton que centraliza TODA la configuración del sistema de clustering
    // Gestiona: Pesos, Normalizaciones, Métricas de Distancia y Habilitación de Variables
    internal class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _instance =
            new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        public static ConfigurationManager Instance => _instance.Value;

        private ConfigurationManager()
        {
            InitializeDefaultConfiguration();
        }

        /// <summary>Vector de pesos para todas las características (numéricas + categóricas)</summary>
        public CustomVector<double> Weights { get; private set; }

        /// <summary>Estrategia de distancia actualmente seleccionada</summary>
        public IDistanceStrategy DistanceStrategy { get; private set; }

        /// <summary>Estrategias de normalización por cada variable numérica (7 variables)</summary>
        private INormalizationStrategy[] _normalizationStrategies;

        /// <summary>Configuración de habilitación y peso por variable</summary>
        private VariableConfigSettings[] _variableSettings;


        private readonly string[] NumericFeatureNames = new string[]
        {
            "Budget",           // 0
            "Popularity",       // 1
            "Revenue",          // 2
            "Runtime",          // 3
            "VoteAverage",      // 4
            "VoteCount",        // 5
            "ReleaseYear"       // 6
        };

        private readonly string[] CategoricalFeatureNames = new string[]
        {
            "Genres",                   
            "Cast",                     
            "Director",                
            "Keywords",                 
            "ProductionCompanies",     
            "SpokenLanguages"           
        };

        // Índices para acceso rápido a settings de variables
        private const int BUDGET_IDX = 0;
        private const int POPULARITY_IDX = 1;
        private const int REVENUE_IDX = 2;
        private const int RUNTIME_IDX = 3;
        private const int VOTEAVERAGE_IDX = 4;
        private const int VOTECOUNT_IDX = 5;
        private const int RELEASEYEAR_IDX = 6;

        private const int GENRES_IDX = 7;
        private const int CAST_IDX = 8;
        private const int DIRECTOR_IDX = 9;
        private const int KEYWORDS_IDX = 10;
        private const int COMPANIES_IDX = 11;
        private const int LANGUAGES_IDX = 12;

        private void InitializeDefaultConfiguration()
        {
            // Inicializar pesos para variables numéricas (7 variables)
            Weights = new CustomVector<double>();
            for (int i = 0; i < 7; i++)
                Weights.Add(0.0);

            // Inicializar estrategias de normalización (MinMax por defecto)
            _normalizationStrategies = new INormalizationStrategy[7];
            for (int i = 0; i < 7; i++)
                _normalizationStrategies[i] = new MinMaxNormalization();

            // Inicializar configuración de variables (13 variables: 7 numéricas + 6 categóricas)
            _variableSettings = new VariableConfigSettings[13];

            // Variables numéricas - habilitadas con peso 0.0
            for (int i = 0; i < 7; i++)
            {
                _variableSettings[i] = new VariableConfigSettings
                {
                    IsEnabled = true,
                    Weight = 0.0,
                    Name = NumericFeatureNames[i]
                };
            }

            // Variables categóricas - habilitadas con peso 0.0
            _variableSettings[GENRES_IDX] = new VariableConfigSettings { IsEnabled = true, Weight = 0.0, Name = "Genres" };
            _variableSettings[CAST_IDX] = new VariableConfigSettings { IsEnabled = true, Weight = 0.0, Name = "Cast" };
            _variableSettings[DIRECTOR_IDX] = new VariableConfigSettings { IsEnabled = true, Weight = 0.0, Name = "Director" };
            _variableSettings[KEYWORDS_IDX] = new VariableConfigSettings { IsEnabled = true, Weight = 0.0, Name = "Keywords" };
            _variableSettings[COMPANIES_IDX] = new VariableConfigSettings { IsEnabled = true, Weight = 0.0, Name = "ProductionCompanies" };
            _variableSettings[LANGUAGES_IDX] = new VariableConfigSettings { IsEnabled = true, Weight = 0.0, Name = "SpokenLanguages" };

            // Métrica de distancia por defecto
            DistanceStrategy = new EuclideanDistance();
        }

        /// <summary>
        /// Expande el vector de pesos para incluir todas las características categóricas
        /// Debe llamarse después de la vectorización cuando se conocen las dimensiones
        /// </summary>
        public void InitializeWeightsForDataset(VectorizationService vectorizer)
        {
            // Agregar pesos para características categóricas
            AddCategoricalWeights(vectorizer.Indexer.GenreIndex.Count, _variableSettings[GENRES_IDX]);
            AddCategoricalWeights(vectorizer.Indexer.CastIndex.Count, _variableSettings[CAST_IDX]);
            AddCategoricalWeights(vectorizer.Indexer.DirectorIndex.Count, _variableSettings[DIRECTOR_IDX]);
            AddCategoricalWeights(vectorizer.Indexer.KeywordsIndex.Count, _variableSettings[KEYWORDS_IDX]);
            AddCategoricalWeights(vectorizer.Indexer.ProductionCompaniesIndex.Count, _variableSettings[COMPANIES_IDX]);
            AddCategoricalWeights(vectorizer.Indexer.SpokenLanguagesIndex.Count, _variableSettings[LANGUAGES_IDX]);
        }

        private void AddCategoricalWeights(int count, VariableConfigSettings settings)
        {
            double weightValue = settings.IsEnabled ? settings.Weight : 0.0;
            for (int i = 0; i < count; i++)
                Weights.Add(weightValue);
        }

        /// <summary>
        /// Cambia la métrica de distancia del sistema
        /// </summary>
        public void SetDistanceMetric(string metricName)
        {
            DistanceStrategy = DistanceFactory.CreateDistance(metricName);
        }


        /// <summary>
        /// Establece la estrategia de normalización para una variable numérica por índice
        /// </summary>
        public void SetNormalizationStrategy(int featureIndex, string strategyName)
        {
            if (featureIndex < 0 || featureIndex >= 7)
                throw new ArgumentOutOfRangeException(nameof(featureIndex),
                    "El índice debe estar entre 0 y 6 (variables numéricas)");

            _normalizationStrategies[featureIndex] = NormalizationFactory.CreateNormalization(strategyName);
        }

        /// <summary>
        /// Establece la estrategia de normalización por nombre de variable
        /// </summary>
        public void SetNormalizationStrategyByName(string featureName, string strategyName)
        {
            int index = Array.IndexOf(NumericFeatureNames, featureName);
            if (index == -1)
                throw new ArgumentException($"Variable '{featureName}' no encontrada. " +
                    $"Variables válidas: {string.Join(", ", NumericFeatureNames)}");

            SetNormalizationStrategy(index, strategyName);
        }

        /// <summary>
        /// Actualiza la configuración completa de una variable (habilitación + peso)
        /// </summary>
        public void UpdateVariableConfig(string variableName, bool isEnabled, double weight)
        {
            int index = GetVariableIndex(variableName);

            _variableSettings[index].IsEnabled = isEnabled;
            _variableSettings[index].Weight = weight;

            // Si es variable numérica, actualizar directamente su peso
            if (index < 7)
            {
                Weights.SetAt(index, isEnabled ? weight : 0.0);
            }
            // Si es categórica, se actualizará en ApplyWeightsToDataset
        }

        public VariableConfigSettings GetVariableConfig(string variableName)
        {
            int index = GetVariableIndex(variableName);
            return _variableSettings[index];
        }

        private int GetVariableIndex(string variableName)
        {
            return variableName.ToLower() switch
            {
                "budget" => BUDGET_IDX,
                "popularity" => POPULARITY_IDX,
                "revenue" => REVENUE_IDX,
                "runtime" => RUNTIME_IDX,
                "voteaverage" => VOTEAVERAGE_IDX,
                "votecount" => VOTECOUNT_IDX,
                "releaseyear" => RELEASEYEAR_IDX,
                "genres" => GENRES_IDX,
                "cast" => CAST_IDX,
                "director" => DIRECTOR_IDX,
                "keywords" => KEYWORDS_IDX,
                "companies" or "productioncompanies" => COMPANIES_IDX,
                "languages" or "spokenlanguages" => LANGUAGES_IDX,
                _ => throw new ArgumentException($"Variable '{variableName}' no reconocida")
            };
        }


        /// <summary>
        /// Actualiza un peso específico por índice directo en el vector de características
        /// </summary>
        public void UpdateWeight(int index, double weight)
        {
            if (index < 0 || index >= Weights.Count)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"El índice debe estar entre 0 y {Weights.Count - 1}");

            Weights.SetAt(index, weight);
        }

        /// <summary>
        /// Aplica los pesos configurados al dataset completo
        /// Debe llamarse después de la normalización
        /// </summary>
        public void ApplyWeightsToDataset(CustomList<Movie> movies, VectorizationService vectorizer)
        {
            // Primero, actualizar pesos categóricos en el vector de Weights
            UpdateCategoricalWeightsInVector(vectorizer);

            // Luego aplicar todos los pesos a las películas
            var iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                Movie movie = iterator.Next();
                CustomVector<double> vector = movie.WeightedFeatureVector;

                for (int i = 0; i < vector.Count && i < Weights.Count; i++)
                {
                    double value = vector.GetAt(i);
                    double weight = Weights.GetAt(i);
                    vector.SetAt(i, value * weight);
                }
            }
        }

        private void UpdateCategoricalWeightsInVector(VectorizationService vectorizer)
        {
            int currentIndex = 7; // Después de las 7 variables numéricas

            // Géneros
            currentIndex = UpdateCategoricalWeightRange(
                currentIndex,
                vectorizer.Indexer.GenreIndex.Count,
                _variableSettings[GENRES_IDX]
            );

            // Cast
            currentIndex = UpdateCategoricalWeightRange(
                currentIndex,
                vectorizer.Indexer.CastIndex.Count,
                _variableSettings[CAST_IDX]
            );

            // Director
            currentIndex = UpdateCategoricalWeightRange(
                currentIndex,
                vectorizer.Indexer.DirectorIndex.Count,
                _variableSettings[DIRECTOR_IDX]
            );

            // Keywords
            currentIndex = UpdateCategoricalWeightRange(
                currentIndex,
                vectorizer.Indexer.KeywordsIndex.Count,
                _variableSettings[KEYWORDS_IDX]
            );

            // Production Companies
            currentIndex = UpdateCategoricalWeightRange(
                currentIndex,
                vectorizer.Indexer.ProductionCompaniesIndex.Count,
                _variableSettings[COMPANIES_IDX]
            );

            // Spoken Languages
            UpdateCategoricalWeightRange(
                currentIndex,
                vectorizer.Indexer.SpokenLanguagesIndex.Count,
                _variableSettings[LANGUAGES_IDX]
            );
        }

        private int UpdateCategoricalWeightRange(int startIndex, int count, VariableConfigSettings settings)
        {
            double weightValue = settings.IsEnabled ? settings.Weight : 0.0;
            for (int i = 0; i < count; i++)
                if (startIndex + i < Weights.Count)
                    Weights.SetAt(startIndex + i, weightValue);

            return startIndex + count;
        }

        /// <summary>
        /// Normaliza todas las variables numéricas del dataset según las estrategias configuradas
        /// </summary>
        public void NormalizeDataset(CustomList<Movie> movies)
        {
            // Paso 1: Calcular estadísticas para cada estrategia
            for (int i = 0; i < 7; i++)
                _normalizationStrategies[i].CalculateStats(movies, i);

            // Paso 2: Aplicar normalización a cada película
            IIterator<Movie> iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                Movie movie = iterator.Next();

                for (int i = 0; i < 7; i++)
                {
                    double originalValue = movie.FeatureVector.GetAt(i);
                    double normalizedValue = _normalizationStrategies[i].Normalize(originalValue);
                    movie.WeightedFeatureVector.SetAt(i, normalizedValue);
                }
            }
        }

    }

    internal class VariableConfigSettings
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public double Weight { get; set; }
    }
}