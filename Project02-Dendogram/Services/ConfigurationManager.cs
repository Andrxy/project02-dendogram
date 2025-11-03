using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Strategies.Normalization;
using Project02_Dendogram.Strategies.Distance;

namespace Project02_Dendogram.Services
{
    internal class ConfigurationManager
    {
        // Singleton simple, thread-safe
        public static readonly ConfigurationManager Instance = new ConfigurationManager();

        // Pesos y estrategias
        public CustomVector<double> Weights { get; private set; }
        public INormalizationStrategy NormalizationStrategy { get; private set; }
        public IDistanceStrategy DistanceStrategy { get; private set; }

        private ConfigurationManager()
        {
            InitializeDefaultWeights();
            NormalizationStrategy = NormalizationFactory.CreateNormalization("minmax");
            DistanceStrategy = DistanceFactory.CreateDistance("euclidean");
        }

        private void InitializeDefaultWeights()
        {
            Weights = new CustomVector<double>();
            Weights.Add(1.0);  // Budget
            Weights.Add(1.0);  // Popularity
            Weights.Add(1.0);  // Revenue
            Weights.Add(1.0);  // Runtime
            Weights.Add(1.0);  // VoteAverage
            Weights.Add(1.0);  // VoteCount
            Weights.Add(1.0);  // ReleaseYear
        }

        public void InitializeWeightsForDataset(VectorizationService vectorizer)
        {
            AddWeights(vectorizer.Indexer.GenreIndex.Count, 15.0);    // Géneros
            AddWeights(vectorizer.Indexer.CastIndex.Count, 1.0);     // Cast
            AddWeights(vectorizer.Indexer.DirectorIndex.Count, 1.0); // Director
            AddWeights(vectorizer.Indexer.KeywordsIndex.Count, 1.0); // Director
            AddWeights(vectorizer.Indexer.ProductionCountriesIndex.Count, 1.0); // Director
            AddWeights(vectorizer.Indexer.SpokenLanguagesIndex.Count, 1.0); // Idiomas

            Console.WriteLine($"Configuración de pesos inicializada:");
            Console.WriteLine($"  Total features: {Weights.Count}");
        }

        private void AddWeights(int count, double weight)
        {
            for (int i = 0; i < count; i++)
                Weights.Add(weight);
        }

        // ==========================
        // Métodos para cambiar configuración (SOLID: Open/Closed)
        // ==========================
        public void UpdateWeight(int index, double weight)
        {
            Weights.SetAt(index, weight);
        }

        public void SetNormalizationStrategy(string name)
        {
            NormalizationStrategy = NormalizationFactory.CreateNormalization(name);
        }

        public void SetDistanceMetric(string name)
        {
            DistanceStrategy = DistanceFactory.CreateDistance(name);
        }

    }
}
