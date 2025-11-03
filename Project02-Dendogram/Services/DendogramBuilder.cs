using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Strategies.Distance;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Services
{
    internal class DendogramBuilder
    {
        private IDistanceStrategy distanceStrategy;

        public DendogramBuilder(IDistanceStrategy distance)
        {
            distanceStrategy = distance;
        }

        public Cluster BuildDendogram(CustomList<Movie> movies)
        {
            // Inicializar clusters hoja
            CustomList<Cluster> clusters = new CustomList<Cluster>();
            IIterator<Movie> it = movies.CreateIterator();
            while (it.HasNext())
            {
                clusters.Add(new Cluster(it.Next()));
            }

            // Clustering jerárquico
            while (clusters.Count > 1)
            {
                double minDistance = double.MaxValue;
                int iMin = -1, jMin = -1;
                int m = clusters.Count;

                // Buscar los dos clusters más cercanos
                for (int i = 0; i < m; ++i)
                {
                    for (int j = i + 1; j < m; ++j)
                    {
                        CustomVector<double> vecI = GetClusterVector(clusters.GetAt(i));
                        CustomVector<double> vecJ = GetClusterVector(clusters.GetAt(j));

                        double d = distanceStrategy.Calculate(vecI, vecJ);
                        if (d < minDistance)
                        {
                            minDistance = d;
                            iMin = i;
                            jMin = j;
                        }
                    }
                }

                // Crear nuevo cluster
                Cluster merged = new Cluster(clusters.GetAt(iMin), clusters.GetAt(jMin), minDistance);

                // Remover correctamente (el índice mayor primero)
                if (iMin > jMin)
                {
                    clusters.RemoveAt(iMin);
                    clusters.RemoveAt(jMin);
                }
                else
                {
                    clusters.RemoveAt(jMin);
                    clusters.RemoveAt(iMin);
                }

                clusters.Add(merged);
            }

            return clusters.GetAt(0);
        }

        // Obtiene un vector representativo de un cluster
        private CustomVector<double> GetClusterVector(Cluster cluster)
        {
            if (cluster.IsLeaf)
                return cluster.Movie.WeightedFeatureVector;

            CustomVector<double> left = GetClusterVector(cluster.Left);
            CustomVector<double> right = GetClusterVector(cluster.Right);

            CustomVector<double> merged = new CustomVector<double>(left.Count);
            for (int i = 0; i < left.Count; i++)
                merged.Add((left.GetAt(i) + right.GetAt(i)) / 2.0);

            return merged;
        }
    }
}
