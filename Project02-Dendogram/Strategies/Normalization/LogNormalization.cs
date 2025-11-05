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
        public void CalculateStats(CustomList<Movie> movies, int featureIndex)
        {
            // No necesita calcular estadísticas previas
        }

        public double Normalize(double value)
        {
            return Math.Log(value + 1);
        }
    }
}