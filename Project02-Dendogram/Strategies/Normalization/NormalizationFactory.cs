using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal class NormalizationFactory
    {
        public static INormalizationStrategy CreateNormalization(string type)
        {
            switch (type.ToLower())
            {
                case "minmax":
                    return new MinMaxNormalization();
                case "zscore":
                    return new ZScoreNormalization();
                case "log":
                    return new LogNormalization();
                default:
                    throw new ArgumentException("Tipo de normalizacion invalida");
            }
        }
    }
}
