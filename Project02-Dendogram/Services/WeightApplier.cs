using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Services
{
    internal class WeightApplier
    {
        public static void ApplyWeights(CustomList<Movie> movies)
        {
            ConfigurationManager config = ConfigurationManager.Instance;
            CustomVector<double> weights = config.Weights;

            IIterator<Movie> iterator = movies.CreateIterator();

            while (iterator.HasNext())
            {
                Movie movie = iterator.Next();
                CustomVector<double> vector = movie.WeightedFeatureVector;

                for (int i = 0; i < vector.Count; i++)
                {
                    double value = vector.GetAt(i);
                    double weight = weights.GetAt(i);
                    vector.SetAt(i, value * weight);
                }
            }
        }
    }
}
