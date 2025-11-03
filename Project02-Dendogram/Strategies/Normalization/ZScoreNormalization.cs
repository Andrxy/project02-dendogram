using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal class ZScoreNormalization : INormalizationStrategy
    {
        private CustomVector<double> means = new CustomVector<double>();
        private CustomVector<double> stdDevs = new CustomVector<double>();
        public void Normalize(CustomList<Movie> movies)
        {
            // Paso 1: calcular medias y desviaciones estándar
            CalculateStats(movies);

            // Paso 2: aplicar estandarización
            Apply(movies);
        }

        private void CalculateStats(CustomList<Movie> movies)
        {
            for (int i = 0; i < 7; i++)
            {
                double sum = 0.0;
                int count = 0;
                var iterator = movies.CreateIterator();
                while (iterator.HasNext())
                {
                    sum += iterator.Next().FeatureVector.GetAt(i);
                    count++;
                }

                double mean = sum / count;
                means.Add(mean);

                // desviación estándar
                double sumSquaredDiff = 0.0;
                iterator.Reset();
                while (iterator.HasNext())
                {
                    double value = iterator.Next().FeatureVector.GetAt(i);
                    sumSquaredDiff += Math.Pow(value - mean, 2);
                }
                double stdDev = Math.Sqrt(sumSquaredDiff / count);
                stdDevs.Add(stdDev);
            }
        }

        private void Apply(CustomList<Movie> movies)
        {
            var iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                Movie movie = iterator.Next();

                for (int i = 0; i < 7; i++)
                {
                    double x = movie.FeatureVector.GetAt(i);
                    double standardized = (x - means.GetAt(i)) / stdDevs.GetAt(i);
                    movie.WeightedFeatureVector.SetAt(i, standardized);
                }
            }
        }
    }
}
