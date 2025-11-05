using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal class NormalizationConfig
    {
        private INormalizationStrategy[] strategies;

        private readonly string[] numericFeatureNames = new string[]
        {
            "Budget",
            "Popularity",
            "Revenue",
            "Runtime",
            "VoteAverage",
            "VoteCount",
            "ReleaseYear"
        };

        public NormalizationConfig()
        {
            strategies = new INormalizationStrategy[7];

            for (int i = 0; i < 7; i++)
            {
                strategies[i] = new MinMaxNormalization();
            }
        }

        public void SetStrategy(int featureIndex, string strategyName)
        {
            if (featureIndex < 0 || featureIndex >= 7)
                throw new ArgumentOutOfRangeException("featureIndex debe estar entre 0 y 6");

            strategies[featureIndex] = NormalizationFactory.CreateNormalization(strategyName);
        }

        public void SetStrategyByName(string featureName, string strategyName)
        {
            int index = Array.IndexOf(numericFeatureNames, featureName);
            if (index == -1)
                throw new ArgumentException($"Variable '{featureName}' no encontrada. Use: {string.Join(", ", numericFeatureNames)}");

            SetStrategy(index, strategyName);
        }

        public void Normalize(CustomList<Movie> movies)
        {
            Console.WriteLine("\n📊 Calculando estadísticas para normalización...");
            for (int i = 0; i < 7; i++)
            {
                strategies[i].CalculateStats(movies, i);
                Console.WriteLine($"  ✓ {numericFeatureNames[i]}: {strategies[i].GetType().Name}");
            }

            Console.WriteLine("\n🔄 Aplicando normalización...");
            var iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                Movie movie = iterator.Next();

                for (int i = 0; i < 7; i++)
                {
                    double originalValue = movie.FeatureVector.GetAt(i);
                    double normalizedValue = strategies[i].Normalize(originalValue);
                    movie.WeightedFeatureVector.SetAt(i, normalizedValue);
                }
            }

            Console.WriteLine("  ✓ Normalización completada");
        }

        public void PrintConfiguration()
        {
            Console.WriteLine("\n📋 Configuración de Normalización:");
            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine($"  {numericFeatureNames[i],-15} → {strategies[i].GetType().Name}");
            }
        }
    }
}