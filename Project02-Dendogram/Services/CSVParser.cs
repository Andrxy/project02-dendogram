using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Services
{
    internal class CSVParser
    {
        private readonly string _filePath;

        public CSVParser(string filePath = null)
        {
            _filePath = filePath ?? @"../../../Resources/test30.csv";
        }

        public CustomList<Movie> ParseMovies()
        {
            CustomList<Movie> movies = new CustomList<Movie>();

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Archivo no encontrado: {_filePath}");

            using (TextFieldParser parser = new TextFieldParser(_filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");       // Separador CSV
                parser.HasFieldsEnclosedInQuotes = true;

                // Saltamos la primera línea si es encabezado
                if (!parser.EndOfData)
                    parser.ReadLine();

                while (!parser.EndOfData)
                {
                    try
                    {
                        string[] fields = parser.ReadFields();
                        Movie movie = ParseLine(fields);
                        movies.Add(movie);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error parseando línea: {ex.Message}");
                    }
                }
            }

            return movies;
        }

        private Movie ParseLine(string[] values)
        {
            Movie movie = new Movie();

            movie.Index = ParseDouble(values[0]);
            movie.Budget = ParseDouble(values[1]);
            movie.Genres = ParseString(values[2]);
            movie.Keywords = ParseString(values[5]);
            movie.Popularity = ParseDouble(values[9]);
            movie.ProductionCompanies = ParseString(values[10]);
            movie.ReleaseYear = ParseYear(values[12]);
            movie.Revenue = ParseDouble(values[13]);
            movie.Runtime = ParseDouble(values[14]);
            movie.SpokenLanguages = ParseString(values[15]);
            movie.Title = values[18];
            movie.VoteAverage = ParseDouble(values[19]);
            movie.VoteCount = ParseDouble(values[20]);
            movie.Cast = ParseString(values[21]);
            movie.Directors = ParseString(values[23]);

            return movie;
        }

        private double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Replace('.', ','); // Ajuste para cultura europea

            if (double.TryParse(value, out double result))
                return result;

            return 0;
        }

        private double ParseYear(string value)
        {
            if (DateTime.TryParse(value, out DateTime date))
                return date.Year;

            return 0;
        }

        private string[] ParseString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Array.Empty<string>();

            // Eliminamos comillas si existen
            value = value.Trim('"');

            string[] split = value.Split(',');
            for (int i = 0; i < split.Length; i++)
                split[i] = split[i].Trim();

            return split;
        }
    }
}
