using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Utils
{
    internal class VectorOperations
    {
        public static double DotProduct(CustomVector<double> v1, CustomVector<double> v2)
        {
            double dotProduct = 0.0;

            IIterator<double> it1 = v1.CreateIterator();
            IIterator<double> it2 = v2.CreateIterator();
            while (it1.HasNext() && it2.HasNext())
            {
                double x = it1.Next();
                double y = it2.Next();
                dotProduct += x * y;
            }

            return dotProduct;
        }
        public static double Norm(CustomVector<double> v)
        {
            double norm = 0.0;

            IIterator<double> it = v.CreateIterator();
            while (it.HasNext())
            {
                double x = it.Next();
                norm += x * x;
            }

            return Math.Sqrt(norm);
        }
        public static CustomVector<double> Substract(CustomVector<double> v1, CustomVector<double> v2)
        {
            CustomVector<double> result = new CustomVector<double>(v1.Count);

            IIterator<double> it1 = v1.CreateIterator();
            IIterator<double> it2 = v2.CreateIterator();
            while (it1.HasNext() && it2.HasNext())
            {
                double x = it1.Next();
                double y = it2.Next();
                result.Add(x - y);
            }

            return result;
        }
    }
}
