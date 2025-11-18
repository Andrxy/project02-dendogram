using System.Windows.Navigation;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Models
{
    public class Movie
    {
        // Datos de Identificacion
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
        public string[] Directors { get; set; }
        public string[] Keywords { get; set; }
        public string[] ProductionCompanies { get; set; }
        public string[] SpokenLanguages { get; set; }
        public CustomVector<double> FeatureVector { get; set; }
        public CustomVector<double> WeightedFeatureVector { get; set; }

        public Movie()
        {
            FeatureVector = new CustomVector<double>();
            WeightedFeatureVector = new CustomVector<double>();
        }

        public double GetNumerical(string variable)
        {
            switch (variable)
            {
                case "budget": return Budget;
                case "popularity": return Popularity;
                case "revenue": return Revenue;
                case "runtime": return Runtime;
                case "voteaverage": return VoteAverage;
                case "votecount": return VoteCount;
                case "releaseyear": return ReleaseYear;
                default: return 0;
            }
        }
        public string[] GetCategorical(string variable)
        {
            switch (variable)
            {
                case "genres": return Genres;
                case "cast": return Cast;
                case "directors": return Directors;
                case "keywords": return Keywords;
                case "companies": return ProductionCompanies;
                case "languages": return SpokenLanguages;
                default: return new string[0];
            }
        }

    }
}