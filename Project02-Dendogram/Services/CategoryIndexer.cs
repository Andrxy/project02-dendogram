using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Services
{
    internal class CategoryIndexer
    {
        public CustomHashMap<string, int> GenreIndex { get; private set; }
        public CustomHashMap<string, int> CastIndex { get; private set; }
        public CustomHashMap<string, int> DirectorIndex { get; private set; }
        public CustomHashMap<string, int> KeywordsIndex { get; private set; }
        public CustomHashMap<string, int> ProductionCountriesIndex { get; private set; }
        public CustomHashMap<string, int> SpokenLanguagesIndex { get; private set; }

        public void BuildIndices(CustomList<Movie> movies)
        {
            GenreIndex = new CustomHashMap<string, int>();
            CastIndex = new CustomHashMap<string, int>();
            DirectorIndex = new CustomHashMap<string, int>();
            KeywordsIndex = new CustomHashMap<string, int>();
            ProductionCountriesIndex = new CustomHashMap<string, int>();
            SpokenLanguagesIndex = new CustomHashMap<string, int>();

            IIterator<Movie> it = movies.CreateIterator();
            while (it.HasNext())
            {
                Movie movie = it.Next();
                AddToIndex(movie.Genres, GenreIndex);
                AddToIndex(movie.Cast, CastIndex);
                AddToIndex(new string[] { movie.Director }, DirectorIndex);
                AddToIndex(movie.Keywords, KeywordsIndex);
                AddToIndex(movie.ProductionCountries, ProductionCountriesIndex);
                AddToIndex(movie.SpokenLanguages, SpokenLanguagesIndex);
            }

            PrintStats();
            PrintAllIndices();
        }

        private void AddToIndex(string[] items, CustomHashMap<string, int> indexMap)
        {
            if (items == null) return;

            foreach (string item in items)
            {
                if (!string.IsNullOrWhiteSpace(item) && !indexMap.ContainsKey(item))
                    indexMap.Put(item, indexMap.Count);
            }
        }

        public void PrintStats()
        {
            Console.WriteLine("📊 === CategoryIndexer Statistics ===");
            Console.WriteLine($"Genres:                {GenreIndex.Count}");
            Console.WriteLine($"Cast members:          {CastIndex.Count}");
            Console.WriteLine($"Directors:             {DirectorIndex.Count}");
            Console.WriteLine($"Keywords:              {KeywordsIndex.Count}");
            Console.WriteLine($"Production countries:  {ProductionCountriesIndex.Count}");
            Console.WriteLine($"Spoken languages:      {SpokenLanguagesIndex.Count}");
            Console.WriteLine("=====================================");
        }

        public void PrintAllIndices()
        {
            Console.WriteLine("🧩 === CategoryIndexer Detailed Contents ===");

            PrintIndex("Genres", GenreIndex);
            PrintIndex("Cast", CastIndex);
            PrintIndex("Directors", DirectorIndex);
            PrintIndex("Keywords", KeywordsIndex);
            PrintIndex("Production Countries", ProductionCountriesIndex);
            PrintIndex("Spoken Languages", SpokenLanguagesIndex);

            Console.WriteLine("============================================");
        }

        private void PrintIndex(string name, CustomHashMap<string, int> index)
        {
            Console.WriteLine($"\n{name} ({index.Count} items):");

            // Si tenés un método tipo GetEntries()
            var it = index.CreateIterator(); // o lo que tengas definido
            while (it.HasNext())
            {
                var entry = it.Next(); // suponiendo que devuelve KeyValuePair<string, int>
                Console.WriteLine($"  [{entry.Value}] {entry.Key}");
            }

            // Alternativamente, si tenés un método GetKeys():
            /*
            var keys = index.GetKeys();
            IIterator<string> it = keys.CreateIterator();
            while (it.HasNext())
            {
                string key = it.Next();
                int value = index.Get(key);
                Console.WriteLine($"  [{value}] {key}");
            }
            */
        }

    }
}
