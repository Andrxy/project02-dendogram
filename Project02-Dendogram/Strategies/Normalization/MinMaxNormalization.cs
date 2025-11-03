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
        private CustomVector<double> mins;
        private CustomVector<double> maxs;
        public void Normalize(CustomList<Movie> movies)
        {
            // Paso 1: Calcular min y max para cada variable numérica
            MinMaxs(movies);

            // Paso 2: Aplicar normalización
            Apply(movies);
        }

        private void MinMaxs(CustomList<Movie> movies)
        {
            mins = new CustomVector<double>();
            maxs = new CustomVector<double>();

            for (int i = 0; i < 7; i++)
            {
                double min = double.MaxValue;
                double max = double.MinValue;

                var iterator = movies.CreateIterator();
                while (iterator.HasNext())
                {
                    double value = iterator.Next().FeatureVector.GetAt(i);
                    if (value < min) min = value;
                    if (value > max) max = value;
                }

                mins.Add(min);
                maxs.Add(max);
            }
        }

        private void Apply(CustomList<Movie> movies)
        {
            var movieIterator = movies.CreateIterator();
            while (movieIterator.HasNext())
            {
                Movie movie = movieIterator.Next();

                for (int i = 0; i < 7; i++)
                {
                    double min = mins.GetAt(i);
                    double max = maxs.GetAt(i);

                    double x = movie.FeatureVector.GetAt(i);

                    if (max - min == 0) 
                        movie.WeightedFeatureVector.SetAt(i, 0);
                    else
                    {
                        double normalized = (x - min) / (max - min);
                        movie.WeightedFeatureVector.SetAt(i, normalized);
                    }

                }
            }
        }
    }
}
