using System.Windows.Navigation;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Models
{
    public class Movie
    {
        // Datos de Identificacion
        public double Index { get; set; }
        public string Title { get; set; }

        // Variables Numericas
        public double Budget { get; set; }
        public double Revenue { get; set; }
        public double Runtime { get; set; }
        public double Popularity { get; set; }
        public double VoteAverage { get; set; }
        public double VoteCount { get; set; }
        public double ReleaseYear { get; set; }

        // Variables Categoricas
        public string[] Genres { get; set; }
        public string[] Cast { get; set; }
        public string Director { get; set; }
        public string[] Keywords { get; set; }
        public string[] ProductionCountries { get; set; }
        public string[] SpokenLanguages { get; set; }
        public CustomVector<double> FeatureVector { get; set; }
        public CustomVector<double> WeightedFeatureVector { get; set; }

        public Movie()
        {
            FeatureVector = new CustomVector<double>();
            WeightedFeatureVector = new CustomVector<double>();
        }

        public override string ToString()
        {
            string safeJoin(string[] arr) => arr == null ? "" : string.Join(" | ", arr);

            return $"Movie: {Title} ({ReleaseYear})\n" +
                   $"Index: {Index}, Budget: {Budget}, Revenue: {Revenue}, Runtime: {Runtime}, Popularity: {Popularity}, VoteAverage: {VoteAverage}, VoteCount: {VoteCount}\n" +
                   $"Genres: {safeJoin(Genres)}\n" +
                   $"Cast: {safeJoin(Cast)}\n" +
                   $"Director: {Director}\n" +
                   $"Keywords: {safeJoin(Keywords)}\n" +
                   $"ProductionCountries: {safeJoin(ProductionCountries)}\n" +
                   $"SpokenLanguages: {safeJoin(SpokenLanguages)}";
        }
    }
}