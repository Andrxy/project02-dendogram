using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Strategies.Distance
{
    internal interface IDistanceStrategy
    {
        double Calculate(CustomVector<double> v1, CustomVector<double> v2);
    }
}
