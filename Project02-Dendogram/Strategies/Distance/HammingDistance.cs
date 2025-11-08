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
    internal class HammingDistance : IDistanceStrategy
    {
        public double Calculate(CustomVector<double> v1, CustomVector<double> v2)
        {
            double distance = 0.0;

            IIterator<double> it1 = v1.CreateIterator();
            IIterator<double> it2 = v2.CreateIterator();
            while (it1.HasNext() && it2.HasNext())
            {
                double x = it1.Next();
                double y = it2.Next();
                double different = (x != y) ? 1.0 : 0.0;
                distance += different;
            }

            return distance;
        }
    }
}
