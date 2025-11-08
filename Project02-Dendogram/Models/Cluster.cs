using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Models {
    internal class Cluster
    {
        public Movie Movie { get; set; }
        public double Distance { get; set; }
        public Cluster Left { get; set; }
        public Cluster Right { get; set; }

        public CustomList<Movie> Movies { get; set; }


        public Cluster(Movie movie) { 
            Movie = movie;
            Distance = 0.0;

            Movies = new CustomList<Movie>();
            Movies.Add(movie);

            Left = null;
            Right = null;
        }
        public Cluster(Cluster left, Cluster right, double distance)
        {
            Movie = null;
            Distance = distance;
            Left = left;
            Right = right;

            Movies = new CustomList<Movie>();
            Movies.Add(left.Movies);
            Movies.Add(right.Movies);
        }
        public bool IsLeaf => Movie != null;
    }
}