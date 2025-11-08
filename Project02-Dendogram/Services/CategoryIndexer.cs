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
        public CustomHashMap<string, int> ProductionCompaniesIndex { get; private set; }
        public CustomHashMap<string, int> SpokenLanguagesIndex { get; private set; }

        public void BuildIndices(CustomList<Movie> movies)
        {
            GenreIndex = new CustomHashMap<string, int>();
            CastIndex = new CustomHashMap<string, int>();
            DirectorIndex = new CustomHashMap<string, int>();
            KeywordsIndex = new CustomHashMap<string, int>();
            ProductionCompaniesIndex = new CustomHashMap<string, int>();
            SpokenLanguagesIndex = new CustomHashMap<string, int>();

            IIterator<Movie> it = movies.CreateIterator();
            while (it.HasNext())
            {
                Movie movie = it.Next();
                AddToIndex(movie.Genres, GenreIndex);
                AddToIndex(movie.Cast, CastIndex);
                AddToIndex(movie.Directors, DirectorIndex);
                AddToIndex(movie.Keywords, KeywordsIndex);
                AddToIndex(movie.ProductionCompanies, ProductionCompaniesIndex);
                AddToIndex(movie.SpokenLanguages, SpokenLanguagesIndex);
            }
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
    }
}
