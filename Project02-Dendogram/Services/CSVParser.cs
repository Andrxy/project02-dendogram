using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Services
{
    internal class CSVParser
    {
        private readonly string _filePath = @"../../../Resources/test30.tsv";
        public CustomList<Movie> ParseMovies()
        {
            CustomList<Movie> movies = new CustomList<Movie>();

            string[] lines = System.IO.File.ReadAllLines(_filePath);

            for (int i = 1; i < lines.Length; ++i)
            {
                Movie movie = ParseLine(lines[i]);
                movies.Add(movie);
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

            // Reemplazar puntos por comas si es necesario (según cultura)
            value = value.Replace('.', ',');

            if (double.TryParse(value, out double result))
                return result;

            return 0;
        }

        private long ParseLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Eliminar espacios y caracteres no numéricos si es necesario
            value = value.Trim();
            value.Replace(".", "");

            if (long.TryParse(value, out long result))
                return result;

            return 0;
        }

        private double ParseYear(string value)
        {
            if (DateTime.TryParse(value, out DateTime date))
                return date.Year;

            // Valor por defecto si está vacío o inválido
            return 0;
        }

        private string[] ParseStringArray(string value)
        {
            string[] split = value.Split(' ');
            return split;
        }

        private string[] ParseJSON(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Array.Empty<string>();

            // Quitar corchetes externos
            string trimmed = value.Trim('[', ']');

            if (string.IsNullOrWhiteSpace(trimmed))
                return Array.Empty<string>();

            // Separar por "}, {" y reconstruir las llaves faltantes
            string[] objects = trimmed
                .Split(new string[] { "}, {" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.StartsWith("{") ? s : "{" + s)
                .Select(s => s.EndsWith("}") ? s : s + "}")
                .ToArray();

            return objects;
        }

    }
}
