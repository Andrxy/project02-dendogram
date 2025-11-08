using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Strategies.Normalization;
using Project02_Dendogram.Strategies.Distance;
using System;

namespace Project02_Dendogram.Services
{
    internal class ConfigurationManager
    {
        public static readonly ConfigurationManager Instance = new ConfigurationManager();

        public CustomVector<double> Weights { get; private set; }
        public NormalizationConfig NormalizationConfig { get; private set; }
        public IDistanceStrategy DistanceStrategy { get; private set; }

        private ConfigurationManager()
        {
            InitializeDefaultWeights();
            NormalizationConfig = new NormalizationConfig();
            DistanceStrategy = DistanceFactory.CreateDistance("euclidean");
        }

        private void InitializeDefaultWeights()
        {
            Weights = new CustomVector<double>();
            Weights.Add(1.0);
            Weights.Add(1.0);
            Weights.Add(1.0);
            Weights.Add(1.0);
            Weights.Add(1.0);
            Weights.Add(1.0);
            Weights.Add(1.0);
        }

        public void InitializeWeightsForDataset(VectorizationService vectorizer)
        {
            AddWeights(vectorizer.Indexer.GenreIndex.Count, 1.0);
            AddWeights(vectorizer.Indexer.CastIndex.Count, 1.0);
            AddWeights(vectorizer.Indexer.DirectorIndex.Count, 1.0);
            AddWeights(vectorizer.Indexer.KeywordsIndex.Count, 1.0);
            AddWeights(vectorizer.Indexer.ProductionCompaniesIndex.Count, 1.0);
            AddWeights(vectorizer.Indexer.SpokenLanguagesIndex.Count, 1.0);
        }

        private void AddWeights(int count, double weight)
        {
            for (int i = 0; i < count; i++)
                Weights.Add(weight);
        }

        public void UpdateWeight(int index, double weight)
        {
            Weights.SetAt(index, weight);
        }

        public void SetNormalizationStrategy(int featureIndex, string strategyName)
        {
            NormalizationConfig.SetStrategy(featureIndex, strategyName);
        }

        public void SetNormalizationStrategyByName(string featureName, string strategyName)
        {
            NormalizationConfig.SetStrategyByName(featureName, strategyName);
        }

        public void SetDistanceMetric(string name)
        {
            DistanceStrategy = DistanceFactory.CreateDistance(name);
        }
    }
}