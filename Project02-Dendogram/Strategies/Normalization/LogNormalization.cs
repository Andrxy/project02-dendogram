using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal class LogNormalization : INormalizationStrategy
    {
        public void Normalize(CustomList<Movie> movies)
        {
            var iterator = movies.CreateIterator();
            while (iterator.HasNext())
            {
                Movie movie = iterator.Next();

                for (int i = 0; i < 7; i++)
                {
                    double x = movie.FeatureVector.GetAt(i);
                    double normalized = Math.Log(x + 1);
                    movie.WeightedFeatureVector.SetAt(i, normalized);
                }
            }
        }
    }
}
