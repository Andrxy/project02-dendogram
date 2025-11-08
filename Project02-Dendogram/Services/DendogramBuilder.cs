using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Strategies.Distance;

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
            // Crear clusters hoja
            CustomList<Cluster> clusters = new CustomList<Cluster>();
            IIterator<Movie> it = movies.CreateIterator();

            while (it.HasNext())
            {
                Movie m = it.Next();
                clusters.Add(new Cluster(m));   // hoja con Movie y Movies.Add(m)
            }

            // Ciclo principal del clustering jerárquico
            while (clusters.Count > 1)
            {
                double minDistance = double.MaxValue;
                int iMin = -1, jMin = -1;
                int m = clusters.Count;

                // Buscar los dos clusters más cercanos
                for (int i = 0; i < m; i++)
                {
                    for (int j = i + 1; j < m; j++)
                    {
                        double d = AverageLinkageDistance(
                            clusters.GetAt(i),
                            clusters.GetAt(j)
                        );

                        if (d < minDistance)
                        {
                            minDistance = d;
                            iMin = i;
                            jMin = j;
                        }
                    }
                }

                // Crear el nuevo cluster fusionado
                Cluster merged = new Cluster(
                    clusters.GetAt(iMin),
                    clusters.GetAt(jMin),
                    minDistance
                );

                // Remover adecuadamente (mayor índice primero)
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

        // ------------------------------------------------------------
        // AVERAGE LINKAGE REAL
        // ------------------------------------------------------------
        private double AverageLinkageDistance(Cluster A, Cluster B)
        {
            double sum = 0.0;
            int count = 0;

            // Iterator for A.Movies
            IIterator<Movie> itA = A.Movies.CreateIterator();
            while (itA.HasNext())
            {
                Movie x = itA.Next();

                // Iterator for B.Movies
                IIterator<Movie> itB = B.Movies.CreateIterator();
                while (itB.HasNext())
                {
                    Movie y = itB.Next();

                    sum += distanceStrategy.Calculate(
                        x.WeightedFeatureVector,
                        y.WeightedFeatureVector
                    );

                    count++;
                }
            }

            return sum / count;
        }
    }
}
