using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Strategies.Distance;
using Project02_Dendogram.Strategies.Normalization;

namespace Project02_Dendogram.Services
{
    internal class ConfigurationManager
    {
        private static ConfigurationManager _instance;

        private ConfigurationManager()
        {
            _normalizationStrategies = new CustomHashMap<string, INormalizationStrategy>();
            _variableSettings = new CustomHashMap<string, VariableConfigSettings>();

            // dejo todo listo con las configuraciones por defecto
            InitializeDefaultConfiguration();
        }

        public static ConfigurationManager GetInstance()
        {
            // singleton de toda la vida
            if (_instance == null)
                _instance = new ConfigurationManager();
            return _instance;
        }

        private CustomHashMap<string, INormalizationStrategy> _normalizationStrategies;
        private CustomHashMap<string, VariableConfigSettings> _variableSettings;

        // estas son las variables numéricas en el orden que van en el vector
        private readonly string[] _numerical = { "budget", "popularity", "revenue", "runtime", "voteaverage", "votecount", "releaseyear"};

        // estas son las categóricas (usan one-hot más adelante)
        private readonly string[] _categorical = { "genres", "cast", "directors", "keywords", "companies", "languages" };

        public IDistanceStrategy DistanceStrategy { get; set; }

        private void InitializeDefaultConfiguration()
        {
            // aquí meto todas las numéricas con normalización por defecto
            foreach (string variable in _numerical)
            {
                _variableSettings.Put(variable, new VariableConfigSettings
                {
                    Name = variable,
                    IsEnabled = true,
                    Weight = 0.0   // que no quede todo en 0
                });

                // normalización por defecto
                _normalizationStrategies.Put(variable, new MinMaxNormalization());
            }

            // lo mismo pero para las categóricas
            foreach (string variable in _categorical)
            {
                _variableSettings.Put(variable, new VariableConfigSettings
                {
                    Name = variable,
                    IsEnabled = true,
                    Weight = 0.0
                });
            }

            // distancia base si el usuario no cambia nada
            DistanceStrategy = new EuclideanDistance();
        }

        public void UpdateVariableConfig(string name, bool isEnabled, double weight)
        {
            if (!_variableSettings.ContainsKey(name))
                throw new Exception("Variable no encontrada: " + name);

            // simplemente actualizo lo que ya existe
            var settings = _variableSettings.Get(name);
            settings.IsEnabled = isEnabled;
            settings.Weight = weight;

            _variableSettings.Put(name, settings);
        }

        public VariableConfigSettings GetVariableConfig(string name)
        {
            return _variableSettings.Get(name);
        }

        public void SetDistanceMetric(string metricName)
        {
            // cambio la distancia según la opción del usuario
            DistanceStrategy = DistanceFactory.CreateDistance(metricName);
        }

        public void SetNormalizationStrategy(string variableName, string strategyName)
        {
            // actualiza la normalización solo de esa variable
            _normalizationStrategies.Put(variableName, NormalizationFactory.CreateNormalization(strategyName));
        }

        // Normaliza solo las numéricas (las categóricas no se normalizan, obvio)
        public void NormalizeDataset(CustomList<Movie> movies)
        {
            // primero calcula los datos que necesita la normalización
            for (int i = 0; i < _numerical.Length; i++)
            {
                string variable = _numerical[i];
                INormalizationStrategy strategy = _normalizationStrategies.Get(variable);
                strategy.CalculateStats(movies, i);
            }

            // ahora se modifica cada película
            IIterator<Movie> it = movies.CreateIterator();
            while (it.HasNext())
            {
                Movie movie = it.Next();

                for (int i = 0; i < _numerical.Length; i++)
                {
                    string variable = _numerical[i];
                    INormalizationStrategy strategy = _normalizationStrategies.Get(variable);

                    double value = movie.FeatureVector.GetAt(i);
                    double normalized = strategy.Normalize(value);

                    movie.WeightedFeatureVector.SetAt(i, normalized);
                }
            }
        }

        // Aquí se aplican los pesos que haya puesto el usuario
        public void ApplyWeightsToDataset(CustomList<Movie> movies, VectorizationService vectorizationService)
        {
            IIterator<Movie> it = movies.CreateIterator();

            while (it.HasNext())
            {
                Movie movie = it.Next();
                CustomVector<double> vector = movie.WeightedFeatureVector;

                int index = 0;

                // primero las numéricas
                foreach (string numVar in _numerical)
                {
                    var config = _variableSettings.Get(numVar);
                    double weight = config.IsEnabled ? config.Weight : 0.0;

                    vector.SetAt(index, vector.GetAt(index) * weight);
                    index++;
                }

                // ahora las categóricas (van en bloques más largos)
                foreach (string variable in _categorical)
                {
                    var config = _variableSettings.Get(variable);
                    double weight = config.IsEnabled ? config.Weight : 0.0;

                    int size = vectorizationService.Indexer._categoryIndices.Get(variable).Count;
                    for (int i = 0; i < size; i++)
                    {
                        double val = vector.GetAt(index + i);
                        vector.SetAt(index + i, val * weight);
                    }

                    index += size;
                }

                movie.WeightedFeatureVector = vector;
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
