using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Utils;

namespace Project02_Dendogram.Strategies.Distance
{
    internal class ManhattanDistance : IDistanceStrategy
    {
        public double Calculate(CustomVector<double> v1, CustomVector<double> v2)
        {
            CustomVector<double> difference = VectorOperations.Substract(v1, v2);
            double distance = 0.0;

            IIterator<double> it = difference.CreateIterator();
            while (it.HasNext())
            {
                double x = it.Next();
                distance += Math.Abs(x);
            }

            return distance;
        }
    }
}
