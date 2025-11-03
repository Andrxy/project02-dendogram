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
    internal class VectorizationService
    {
        private readonly CategoryIndexer _indexer;
        private readonly MovieVectorizer _vectorizer;

        public VectorizationService()
        {
            _indexer = new CategoryIndexer();
            _vectorizer = new MovieVectorizer(_indexer);
        }

        public void VectorizeMovies(CustomList<Movie> movies)
        {
            // Paso 1: construir índices
            _indexer.BuildIndices(movies);

            // Paso 2: vectorizar todas las películas
            IIterator<Movie> it = movies.CreateIterator();

            while (it.HasNext())
            {
                Movie movie = it.Next();
                movie.FeatureVector = _vectorizer.Vectorize(movie);
                movie.WeightedFeatureVector = CopyVector(movie.FeatureVector);
            }
        }

        private CustomVector<double> CopyVector(CustomVector<double> v)
        {
            CustomVector<double> r = new CustomVector<double>(v.Count);
            IIterator<double> it = v.CreateIterator();
            while (it.HasNext())
            {
                r.Add(it.Next());
            }

            return r;
        }

        public CategoryIndexer Indexer => _indexer;
    }
}
