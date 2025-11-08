using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Strategies.Normalization;

namespace Project02_Dendogram.Strategies.Distance
{
    internal class DistanceFactory
    {
        public static IDistanceStrategy CreateDistance(string type)
        {
            switch (type.ToLower())
            {
                case "euclidean":
                    return new EuclideanDistance();
                case "cosine":
                    return new CosineDistance();
                case "manhattan":
                    return new ManhattanDistance();
                case "hamming":
                    return new HammingDistance();
                default:
                    throw new ArgumentException("Tipo de normalizacion invalida");
            }
        }
    }
}
