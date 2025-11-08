using System;
using System.IO;
using System.Linq;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Services
{
    internal class CSVParser
    {
        private readonly string _filePath;

        // Constructor que acepta una ruta personalizada
        public CSVParser(string filePath = null)
        {
            _filePath = filePath ?? @"../../../Resources/test30.tsv";
        }

        public CustomList<Movie> ParseMovies()
        {
            CustomList<Movie> movies = new CustomList<Movie>();

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Archivo no encontrado: {_filePath}");
            }

            string[] lines = File.ReadAllLines(_filePath);

            if (lines.Length <= 1)
            {
                throw new InvalidOperationException("El archivo está vacío o solo contiene encabezados");
            }

            for (int i = 1; i < lines.Length; ++i)
            {
                try
                {
                    Movie movie = ParseLine(lines[i]);
                    movies.Add(movie);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error parseando línea {i}: {ex.Message}");
                }
            }

            return movies;
        }

        private Movie ParseLine(string line)
        {
            string[] values = line.Split('\t');
            Movie movie = new Movie();

            movie.Index = ParseDouble(values[0]);
            movie.Budget = ParseDouble(values[1]);
            movie.Genres = ParseStringArray(values[2]);
            movie.Keywords = ParseStringArray(values[5]);
            movie.Popularity = ParseLong(values[9]);
            movie.ProductionCountries = ParseJSON(values[11]);
            movie.ReleaseYear = ParseYear(values[12]);
            movie.Revenue = ParseDouble(values[13]);
            movie.Runtime = ParseDouble(values[14]);
            movie.SpokenLanguages = ParseJSON(values[15]);
            movie.Title = values[18];
            movie.VoteAverage = ParseDouble(values[19]);
            movie.VoteCount = ParseDouble(values[20]);
            movie.Cast = ParseStringArray(values[21]);
            movie.Director = values[23];

            return movie;
        }

        private double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Replace('.', ',');

            if (double.TryParse(value, out double result))
                return result;

            return 0;
        }

        private long ParseLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Trim().Replace(".", "");

            if (long.TryParse(value, out long result))
                return result;

            return 0;
        }

        private double ParseYear(string value)
        {
            if (DateTime.TryParse(value, out DateTime date))
                return date.Year;

            return 0;
        }

        private string[] ParseStringArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Array.Empty<string>();

            string[] split = value.Split(' ');
            return split;
        }

        private string[] ParseJSON(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Array.Empty<string>();

            string trimmed = value.Trim('[', ']');

            if (string.IsNullOrWhiteSpace(trimmed))
                return Array.Empty<string>();

            string[] objects = trimmed
                .Split(new string[] { "}, {" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.StartsWith("{") ? s : "{" + s)
                .Select(s => s.EndsWith("}") ? s : s + "}")
                .ToArray();

            return objects;
        }
    }
}