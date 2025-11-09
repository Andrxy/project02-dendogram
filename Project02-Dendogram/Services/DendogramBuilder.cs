using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Strategies.Distance;

namespace Project02_Dendogram.Services
{
    internal class DendogramBuilder
    {
        private readonly IDistanceStrategy _strategy;

        public DendogramBuilder(IDistanceStrategy strategy)
        {
            _strategy = strategy;
        }

        public Cluster BuildDendogram(CustomList<Movie> movies)
        {
            var matrix = BuildDistanceMatrix(movies);
            var clusters = InitializeClusters(movies);
            return RunHAC(clusters, matrix);
        }

        private CustomMatrix<double> BuildDistanceMatrix(CustomList<Movie> movies)
        {
            int n = movies.Count;
            CustomMatrix<double> M = new CustomMatrix<double>(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        M.SetAt(i, j, 0);
                    }
                    else if (j < i)
                    {
                        M.SetAt(i, j, M.GetAt(j, i));
                    }
                    else
                    {
                        double d = _strategy.Calculate(
                            movies.GetAt(i).WeightedFeatureVector,
                            movies.GetAt(j).WeightedFeatureVector
                        );
                        M.SetAt(i, j, d);
                    }
                }
            }

            return M;
        }

        private CustomList<Cluster> InitializeClusters(CustomList<Movie> movies)
        {
            CustomList<Cluster> clusters = new CustomList<Cluster>();

            for (int i = 0; i < movies.Count; i++)
                clusters.Add(new Cluster(movies.GetAt(i), i));

            return clusters;
        }

        private Cluster RunHAC(CustomList<Cluster> clusters, CustomMatrix<double> M)
        {
            while (clusters.Count > 1)
            {
                var (idxA, idxB, dist) = FindClosestClusters(M, clusters.Count);

                Cluster merged = new Cluster(
                    clusters.GetAt(idxA),
                    clusters.GetAt(idxB),
                    dist
                );

                CustomMatrix<double> newMatrix = RebuildMatrix(
                    M, clusters, idxA, idxB, merged
                );

                clusters = RebuildClusterList(clusters, idxA, idxB, merged);

                M = newMatrix;
            }

            return clusters.GetAt(0);
        }

        private (int idxA, int idxB, double dist) FindClosestClusters(
            CustomMatrix<double> M, int size)
        {
            double minDist = double.MaxValue;
            int idxA = -1, idxB = -1;

            for (int i = 0; i < size; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    double d = M.GetAt(i, j);
                    if (d < minDist)
                    {
                        minDist = d;
                        idxA = i;
                        idxB = j;
                    }
                }
            }

            return (idxA, idxB, minDist);
        }

        // ===================================================================
        // CORRECCIÓN PRINCIPAL: Acceso correcto a la matriz simétrica
        // ===================================================================
        private CustomMatrix<double> RebuildMatrix(
            CustomMatrix<double> oldM,
            CustomList<Cluster> clusters,
            int idxA,
            int idxB,
            Cluster merged)
        {
            int oldSize = clusters.Count;
            int newSize = oldSize - 1;
            CustomMatrix<double> newM = new CustomMatrix<double>(newSize);

            // Asegurar que idxA < idxB para simplificar la lógica
            if (idxA > idxB)
            {
                int temp = idxA;
                idxA = idxB;
                idxB = temp;
            }

            // ---- Nueva fila/columna 0 (cluster merged) ----
            int col = 1;

            for (int k = 0; k < oldSize; k++)
            {
                if (k == idxA || k == idxB) continue;

                double dist = ComputeNewClusterDistance(
                    oldM,
                    idxA,
                    idxB,
                    k,
                    clusters.GetAt(idxA).Indexes.Count,
                    clusters.GetAt(idxB).Indexes.Count
                );

                newM.SetAt(0, col, dist);
                newM.SetAt(col, 0, dist);
                col++;
            }

            // ---- Copiar submatriz (clusters no fusionados) ----
            int newRow = 1;

            for (int i = 0; i < oldSize; i++)
            {
                if (i == idxA || i == idxB) continue;

                int newCol = 1;

                for (int j = 0; j < oldSize; j++)
                {
                    if (j == idxA || j == idxB) continue;

                    newM.SetAt(newRow, newCol, oldM.GetAt(i, j));
                    newCol++;
                }

                newRow++;
            }

            return newM;
        }

        private CustomList<Cluster> RebuildClusterList(
            CustomList<Cluster> clusters,
            int idxA,
            int idxB,
            Cluster merged)
        {
            CustomList<Cluster> newList = new CustomList<Cluster>();
            newList.Add(merged);

            for (int i = 0; i < clusters.Count; i++)
            {
                if (i != idxA && i != idxB)
                    newList.Add(clusters.GetAt(i));
            }

            return newList;
        }

        // ===================================================================
        // CORRECCIÓN: Acceso correcto a matriz simétrica
        // ===================================================================
        private double ComputeNewClusterDistance(
            CustomMatrix<double> M,
            int idxA,
            int idxB,
            int k,
            int sizeA,
            int sizeB)
        {
            // Acceso simétrico: siempre usar (min, max) para evitar errores
            double dA = GetSymmetricDistance(M, idxA, k);
            double dB = GetSymmetricDistance(M, idxB, k);

            return (sizeA * dA + sizeB * dB) / (sizeA + sizeB);
        }

        // Método auxiliar para acceder correctamente a la matriz simétrica
        private double GetSymmetricDistance(CustomMatrix<double> M, int i, int j)
        {
            if (i == j)
                return 0;

            // Siempre acceder a la parte superior de la matriz simétrica
            if (i < j)
                return M.GetAt(i, j);
            else
                return M.GetAt(j, i);
        }
    }
}