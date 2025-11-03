using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Utils;

namespace Project02_Dendogram.Strategies.Distance
{
    internal class CosineDistance : IDistanceStrategy
    {
        public double Calculate(CustomVector<double> v1, CustomVector<double> v2)
        {
            double dotProduct = VectorOperations.DotProduct(v1, v2);
            double norm1 = VectorOperations.Norm(v1);
            double norm2 = VectorOperations.Norm(v2);

            return 1 - dotProduct / norm1 * norm2; 
        }
    }
}
