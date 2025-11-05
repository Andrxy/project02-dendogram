using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Normalization
{
    internal interface INormalizationStrategy
    {
        // Calcula estadísticas necesarias para la normalización (min, max, mean, etc.)
        void CalculateStats(CustomList<Movie> movies, int featureIndex);

        // Normaliza un valor individual
        double Normalize(double value);
    }
}