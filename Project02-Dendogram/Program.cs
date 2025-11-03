using System;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Services;

namespace Project02_Dendogram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("🎬 Iniciando carga de datos desde movies.tsv...\n");

            // 1️⃣ Parsear archivo TSV
            CSVParser parser = new CSVParser();
            CustomList<Movie> movies = parser.ParseMovies();
            Console.WriteLine($"✅ Películas cargadas: {movies.Count}\n");

            // 2️⃣ Vectorizar películas
            VectorizationService vectorization = new VectorizationService();
            vectorization.VectorizeMovies(movies);

            // 3️⃣ Normalizar y ponderar
            ConfigurationManager config = ConfigurationManager.Instance;
            config.InitializeWeightsForDataset(vectorization);
            config.NormalizationStrategy.Normalize(movies);
            WeightApplier.ApplyWeights(movies);

            // 4️⃣ Mostrar vectores ponderados
            Console.WriteLine("\n🎞️ Vectores ponderados:");
            var it1 = movies.CreateIterator();
            while (it1.HasNext())
            {
                Console.WriteLine(it1.Next().WeightedFeatureVector.ToString());
            }

            // 5️⃣ Construir dendrograma
            Console.WriteLine("\n🌳 Construyendo dendrograma...");
            DendogramBuilder b = new DendogramBuilder(config.DistanceStrategy);
            Cluster root = b.BuildDendogram(movies);

            // 6️⃣ Imprimir dendrograma textual
            Console.WriteLine("\n🧩 Dendrograma textual:");
            PrintDendrogram(root, "");

            Console.WriteLine("\n✅ Debug del dendrograma completado.");
            Console.ReadLine();
        }

        // Método recursivo para imprimir el dendrograma
        static void PrintDendrogram(Cluster node, string indent)
        {
            if (node == null) return;

            if (node.IsLeaf)
            {
                Console.WriteLine($"{indent}- {node.Movie.Title}");
            }
            else
            {
                Console.WriteLine($"{indent}+ Merge (distancia: {node.Distance:0.00})");
                PrintDendrogram(node.Left, indent + "  ");
                PrintDendrogram(node.Right, indent + "  ");
            }
        }
    }
}
