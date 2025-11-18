using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Strategies.Distance;

namespace Project02_Dendogram.Services
{
    internal class DendogramBuilder
    {
        private readonly IDistanceStrategy _distanceStrategy;

        public DendogramBuilder(IDistanceStrategy strategy)
        {
            _distanceStrategy = strategy;
        }

        public Cluster BuildDendogram(CustomList<Movie> movies)
        {
            // matriz inicial NxN
            CustomMatrix<double> matrix = BuildDistanceMatrix(movies);

            // Clusters individuales
            CustomList<Cluster> clusters = InitializeClusters(movies);

            while (clusters.Count > 1)
            {
                // Encontrar los dos clusters más cercanos
                var (a, b, dist) = FindClosestClusters(matrix, clusters.Count);

                // Fusionar
                Cluster merged = new Cluster(clusters.GetAt(a), clusters.GetAt(b), dist);

                // Reemplazar el cluster de menor índice por el merged
                int keep = Math.Min(a, b);
                int drop = Math.Max(a, b);

                // Crear nueva matriz actualizada
                matrix = UpdateMatrix(matrix, clusters, keep, drop);

                // Actualizar la lista de clusters
                clusters.SetAt(keep, merged);   // merged va en el índice correcto
                clusters.RemoveAt(drop);         // se elimina el que sobra
            }

            return clusters.GetAt(0);
        }


        private CustomMatrix<double> BuildDistanceMatrix(CustomList<Movie> movies)
        {
            int n = movies.Count;
            var matrix = new CustomMatrix<double>(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if (i == j)
                    {
                        matrix.SetAt(i, j, 0);
                        continue;
                    }

                    double d = _distanceStrategy.Calculate(
                        movies.GetAt(i).WeightedFeatureVector,
                        movies.GetAt(j).WeightedFeatureVector
                    );

                    matrix.SetAt(i, j, d);
                    matrix.SetAt(j, i, d);
                }
            }

            return matrix;
        }


        private CustomList<Cluster> InitializeClusters(CustomList<Movie> movies)
        {
            var clusters = new CustomList<Cluster>();
            for (int i = 0; i < movies.Count; i++)
                clusters.Add(new Cluster(movies.GetAt(i), i));
            return clusters;
        }


        private (int, int, double) FindClosestClusters(CustomMatrix<double> matrix, int size)
        {
            double minDist = double.MaxValue;
            int a = -1, b = -1;

            for (int i = 0; i < size; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    double d = matrix.GetAt(i, j);
                    if (d < minDist)
                    {
                        minDist = d;
                        a = i;
                        b = j;
                    }
                }
            }

            return (a, b, minDist);
        }


        private CustomMatrix<double> UpdateMatrix(CustomMatrix<double> oldMatrix, CustomList<Cluster> clusters, int keep, int drop)
        {
            int oldN = clusters.Count;
            int newN = oldN - 1;

            var newMatrix = new CustomMatrix<double>(newN);

            // Mapear índices: oldIndex -> newIndex
            int[] oldToNew = new int[oldN];
            int newIdx = 0;
            for (int i = 0; i < oldN; i++)
            {
                if (i == drop)
                {
                    oldToNew[i] = -1; // Este índice se elimina
                }
                else
                {
                    oldToNew[i] = newIdx;
                    newIdx++;
                }
            }

            // ---------------------------------------------------------------------
            // 1) Copiar distancias que NO involucran keep ni drop
            // ---------------------------------------------------------------------
            for (int i = 0; i < oldN; i++)
            {
                if (i == keep || i == drop) continue;

                for (int j = i + 1; j < oldN; j++)
                {
                    if (j == keep || j == drop) continue;

                    int ni = oldToNew[i];
                    int nj = oldToNew[j];

                    double dist = oldMatrix.GetAt(i, j);
                    newMatrix.SetAt(ni, nj, dist);
                    newMatrix.SetAt(nj, ni, dist);
                }
            }

            // ---------------------------------------------------------------------
            // 2) Calcular distancias del nuevo merged cluster (Average Linkage)
            // ---------------------------------------------------------------------
            int mergedNewIdx = oldToNew[keep];
            int sizeKeep = clusters.GetAt(keep).Indexes.Count;
            int sizeDrop = clusters.GetAt(drop).Indexes.Count;
            int totalSize = sizeKeep + sizeDrop;

            for (int i = 0; i < oldN; i++)
            {
                if (i == keep || i == drop) continue;

                // Average Linkage: promedio ponderado de distancias
                double distKeep = oldMatrix.GetAt(keep, i);
                double distDrop = oldMatrix.GetAt(drop, i);

                double newDist = (sizeKeep * distKeep + sizeDrop * distDrop) / totalSize;

                int ni = oldToNew[i];
                newMatrix.SetAt(mergedNewIdx, ni, newDist);
                newMatrix.SetAt(ni, mergedNewIdx, newDist);
            }

            // Diagonal = 0
            newMatrix.SetAt(mergedNewIdx, mergedNewIdx, 0);

            return newMatrix;
        }
    }
}