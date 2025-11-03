using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            vector.Add(movie.Budget);
            vector.Add(movie.Popularity);
            vector.Add(movie.Revenue);
            vector.Add(movie.Runtime);
            vector.Add(movie.VoteAverage);
            vector.Add(movie.VoteCount);
            vector.Add(movie.ReleaseYear);

            // --- Categóricas ---
            vector.Append(CreateOneHot(movie.Genres, _indexer.GenreIndex));
            vector.Append(CreateOneHot(movie.Cast, _indexer.CastIndex));
            vector.Append(CreateOneHot(new string[] { movie.Director }, _indexer.DirectorIndex));
            vector.Append(CreateOneHot(movie.Keywords, _indexer.KeywordsIndex));
            vector.Append(CreateOneHot(movie.ProductionCountries, _indexer.ProductionCountriesIndex));
            vector.Append(CreateOneHot(movie.SpokenLanguages, _indexer.SpokenLanguagesIndex));

            return vector;
        }

        private CustomVector<double> CreateOneHot(string[] items, CustomHashMap<string, int> map)
        {
            CustomVector<double> oneHot = new CustomVector<double>(map.Count);
            for (int i = 0; i < map.Count; i++) oneHot.Add(0);

            if (items == null) return oneHot;

            foreach (string item in items)
            {
                if (map.ContainsKey(item))
                    oneHot.SetAt(map.Get(item), 1);
            }

            return oneHot;
        }
    }
}
