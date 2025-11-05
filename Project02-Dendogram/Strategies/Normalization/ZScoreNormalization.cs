using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal class ZScoreNormalization : INormalizationStrategy
    {
        private double mean;
        private double stdDev;

        public void CalculateStats(CustomList<Movie> movies, int featureIndex)
        {
            // Calcular media
            double sum = 0.0;
            int count = 0;
            var iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                sum += iterator.Next().FeatureVector.GetAt(featureIndex);
                count++;
            }

            mean = sum / count;

            // Calcular desviación estándar
            double sumSquaredDiff = 0.0;
            iterator.Reset();
            while (iterator.HasNext())
            {
                double value = iterator.Next().FeatureVector.GetAt(featureIndex);
                sumSquaredDiff += Math.Pow(value - mean, 2);
            }
            stdDev = Math.Sqrt(sumSquaredDiff / count);
        }

        public double Normalize(double value)
        {
            if (stdDev == 0)
                return 0;

            return (value - mean) / stdDev;
        }
    }
}