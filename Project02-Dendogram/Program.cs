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

            // 3️⃣ Configurar normalización personalizada
            ConfigurationManager config = ConfigurationManager.Instance;
            config.InitializeWeightsForDataset(vectorization);

            // ⚙️ CONFIGURAR ESTRATEGIAS DE NORMALIZACIÓN PERSONALIZADAS
            // Puedes configurar cada variable numérica con una estrategia diferente:

            // Opción 1: Por índice (0-6)
            config.SetNormalizationStrategy(0, "log");      // Budget → Log
            config.SetNormalizationStrategy(1, "minmax");   // Popularity → MinMax
            config.SetNormalizationStrategy(2, "log");      // Revenue → Log

            // Opción 2: Por nombre de variable (más legible)
            config.SetNormalizationStrategyByName("Runtime", "minmax");
            config.SetNormalizationStrategyByName("VoteAverage", "zscore");
            config.SetNormalizationStrategyByName("VoteCount", "log");
            config.SetNormalizationStrategyByName("ReleaseYear", "minmax");

            // Mostrar configuración
            config.PrintConfiguration();

            // 4️⃣ Aplicar normalización y ponderación
            config.NormalizationConfig.Normalize(movies);
            WeightApplier.ApplyWeights(movies);

            // 5️⃣ Mostrar algunos vectores ponderados
            Console.WriteLine("\n🎞️ Primeros 3 vectores ponderados:");
            var it1 = movies.CreateIterator();
            int count = 0;
            while (it1.HasNext() && count < 3)
            {
                Movie m = it1.Next();
                Console.WriteLine($"{m.Title}: {m.WeightedFeatureVector.ToString()}");
                count++;
            }

            // 6️⃣ Construir dendrograma
            Console.WriteLine("\n🌳 Construyendo dendrograma...");
            DendogramBuilder b = new DendogramBuilder(config.DistanceStrategy);
            Cluster root = b.BuildDendogram(movies);

            // 7️⃣ Imprimir dendrograma textual
            Console.WriteLine("\n🧩 Dendrograma textual:");
            PrintDendrogram(root, "");

            Console.WriteLine("\n✅ Proceso completado.");
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