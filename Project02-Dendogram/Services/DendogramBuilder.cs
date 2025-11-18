using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Strategies.Distance;

namespace Project02_Dendogram.Services
{
    internal class DendogramBuilder
    {
        // Estrategia de distancia que usaremos para calcular similitud entre películas
        private readonly IDistanceStrategy _distanceStrategy;

        public DendogramBuilder(IDistanceStrategy strategy)
        {
            _distanceStrategy = strategy;
        }

        // Construye el dendograma completo a partir de la lista de películas
        public Cluster BuildDendogram(CustomList<Movie> movies)
        {
            // Primero calculamos la matriz de distancias
            CustomMatrix<double> matrix = BuildDistanceMatrix(movies);

            // Inicializamos cada película como un cluster individual
            CustomList<Cluster> clusters = InitializeClusters(movies);

            // Repetimos hasta que quede un solo cluster
            while (clusters.Count > 1)
            {
                // Encontramos los dos clusters más cercanos
                var (a, b, dist) = FindClosestClusters(matrix, clusters.Count);

                // Los fusionamos en un nuevo cluster
                var merged = new Cluster(clusters.GetAt(a), clusters.GetAt(b), dist);

                // Actualizamos la matriz de distancias con el cluster fusionado
                matrix = UpdateMatrix(matrix, clusters, a, b, merged);

                // Actualizamos la lista de clusters
                clusters = UpdateClusters(clusters, a, b, merged);
            }

            // Devuelve el cluster final que representa todo el dendograma
            return clusters.GetAt(0);
        }

        // Construye la matriz de distancias entre todas las películas
        private CustomMatrix<double> BuildDistanceMatrix(CustomList<Movie> movies)
        {
            int n = movies.Count;
            var matrix = new CustomMatrix<double>(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue; // La distancia de un cluster consigo mismo siempre es 0

                    // Como la matriz es simétrica, reutilizamos la distancia calculada si j<i
                    double distance = j < i ? matrix.GetAt(j, i) : _distanceStrategy.Calculate(movies.GetAt(i).WeightedFeatureVector, movies.GetAt(j).WeightedFeatureVector);

                    matrix.SetAt(i, j, distance);
                }
            }

            return matrix;
        }

        // Inicializa cada película como un cluster individual
        private CustomList<Cluster> InitializeClusters(CustomList<Movie> movies)
        {
            CustomList<Cluster> clusters = new CustomList<Cluster>();
            for (int i = 0; i < movies.Count; i++)
                clusters.Add(new Cluster(movies.GetAt(i), i));
            return clusters;
        }

        // Encuentra los dos clusters más cercanos en la matriz de distancias
        private (int, int, double) FindClosestClusters(CustomMatrix<double> matrix, int size)
        {
            double minDist = double.MaxValue;
            int idxA = -1, idxB = -1;

            for (int i = 0; i < size; i++)
            {
                for (int j = i + 1; j < size; j++)
                {
                    if (matrix.GetAt(i, j) < minDist)
                    {
                        minDist = matrix.GetAt(i, j);
                        idxA = i;
                        idxB = j;
                    }
                }
            }

            return (idxA, idxB, minDist);
        }

        // Actualiza la matriz de distancias después de fusionar dos clusters
        private CustomMatrix<double> UpdateMatrix(CustomMatrix<double> oldMatrix, CustomList<Cluster> clusters, int a, int b, Cluster merged)
        {
            int newSize = clusters.Count - 1;
            var newMatrix = new CustomMatrix<double>(newSize);

            // Calculamos las distancias del nuevo cluster fusionado con los demás clusters
            int colIndex = 1;
            for (int k = 0; k < clusters.Count; k++)
            {
                if (k == a || k == b) continue;

                double distamce = (clusters.GetAt(a).Indexes.Count * GetDistance(oldMatrix, a, k)
                             + clusters.GetAt(b).Indexes.Count * GetDistance(oldMatrix, b, k))
                              / (clusters.GetAt(a).Indexes.Count + clusters.GetAt(b).Indexes.Count);

                newMatrix.SetAt(0, colIndex, distamce);
                newMatrix.SetAt(colIndex, 0, distamce);
                colIndex++;
            }

            // Copiamos las distancias de los clusters que no fueron fusionados
            int rowIndex = 1;
            for (int i = 0; i < clusters.Count; i++)
            {
                if (i == a || i == b) continue;

                int col = 1;
                for (int j = 0; j < clusters.Count; j++)
                {
                    if (j == a || j == b) continue;
                    newMatrix.SetAt(rowIndex, col++, oldMatrix.GetAt(i, j));
                }
                rowIndex++;
            }

            return newMatrix;
        }

        // Actualiza la lista de clusters después de la fusión
        private CustomList<Cluster> UpdateClusters(CustomList<Cluster> clusters, int a, int b, Cluster merged)
        {
            if (a > b)
            {
                clusters.RemoveAt(a);
                clusters.RemoveAt(b);
            }
            else
            {
                clusters.RemoveAt(b);
                clusters.RemoveAt(a);
            }
            clusters.Add(merged);
            return clusters;
        }

        // Helper para acceder correctamente a la matriz simétrica
        private double GetDistance(CustomMatrix<double> matrix, int i, int j)
        {
            return i < j ? matrix.GetAt(i, j) : matrix.GetAt(j, i);
        }
    }
}
