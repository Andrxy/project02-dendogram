using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal class MinMaxNormalization : INormalizationStrategy
    {
        private double min;
        private double max;

        public void CalculateStats(CustomList<Movie> movies, int featureIndex)
        {
            min = double.MaxValue;
            max = double.MinValue;

            var iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                double value = iterator.Next().FeatureVector.GetAt(featureIndex);
                if (value < min) min = value;
                if (value > max) max = value;
            }
        }

        public double Normalize(double value)
        {
            if (max - min == 0)
                return 0;

            return (value - min) / (max - min);
        }
    }
}