using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Services
{
    internal class MovieVectorizer
    {
        private readonly CategoryIndexer _indexer;

        public MovieVectorizer(CategoryIndexer indexer)
        {
            _indexer = indexer;
        }

        public CustomVector<double> Vectorize(Movie movie)
        {
            CustomVector<double> vector = new CustomVector<double>();

            // --- Numéricas ---
            string[] numerical = { "budget", "popularity", "revenue", "runtime", "voteaverage", "votecount", "releaseyear" };
            foreach (string variable in numerical)
                vector.Add(movie.GetNumerical(variable));

            // --- Categóricas ---
            string[] categorical = { "genres", "cast", "directors", "keywords", "companies", "languages" };
            foreach (string variable in categorical)
            {
                CustomHashMap<string, int> map = _indexer.GetIndexMap(variable);
                string[] items = movie.GetCategorical(variable);
                vector.Append(CreateOneHot(items, map));
            }

            return vector;
        }

        private CustomVector<double> CreateOneHot(string[] items, CustomHashMap<string, int> hashmap)
        {
            CustomVector<double> oneHot = new CustomVector<double>();
            for (int i = 0; i < hashmap.Count; i++) oneHot.Add(0);

            if (items == null) return oneHot;

            foreach (string item in items)
            {
                if (hashmap.ContainsKey(item))
                    oneHot.SetAt(hashmap.Get(item), 1);
            }

            return oneHot;
        }
    }
}
