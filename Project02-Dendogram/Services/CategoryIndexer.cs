using System.Text;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Services
{
    internal class CategoryIndexer
    {
        // Diccionario general: nombre de categoría mapeado su mapa de índices
        public CustomHashMap<string, CustomHashMap<string, int>> _categoryIndices {  get; set; }
        private readonly string[] _categorical = { "genres", "cast", "directors", "keywords", "companies", "languages" };


        public CategoryIndexer()
        {
            _categoryIndices = new CustomHashMap<string, CustomHashMap<string, int>>();

            // Crear los mapas vacíos para cada categoría
            foreach (string variable in _categorical)
                _categoryIndices.Put(variable, new CustomHashMap<string, int>());
        }

        public void BuildIndices(CustomList<Movie> movies)
        {
            IIterator<Movie> it = movies.CreateIterator();

            while (it.HasNext())
            {
                Movie movie = it.Next();
                foreach (string variable in _categorical)
                    AddToIndex(movie.GetCategorical(variable), _categoryIndices.Get(variable));
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Stats de categorías ===");

            foreach (string variable in _categorical)
            {
                var map = _categoryIndices.Get(variable);
                sb.AppendLine($"{variable}: {map.Count} elementos");
            }

            System.Windows.MessageBox.Show(sb.ToString(), "Stats de categorías");

        }

        private void AddToIndex(string[] items, CustomHashMap<string, int> indexMap)
        {
            if (items == null) return;

            foreach (string item in items)
            {
                if (!string.IsNullOrWhiteSpace(item) && !indexMap.ContainsKey(item))
                {
                    int newIndex = indexMap.Count;
                    indexMap.Put(item, newIndex);
                }
            }
        }

        // obtener cualquier mapa
        public CustomHashMap<string, int> GetIndexMap(string categoryName)
        {
            if (_categoryIndices.ContainsKey(categoryName))
                return _categoryIndices.Get(categoryName);

            throw new Exception($"Índice no encontrado para la categoría '{categoryName}'.");
        }
    }
}
