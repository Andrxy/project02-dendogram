using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project02_Dendogram.Models
{
    internal class Cluster
    {
        public Movie Movie { get; set; }
        public double Distance { get; set; }
        public Cluster Left { get; set; }
        public Cluster Right { get; set; }

        public Cluster(Movie movie) {
            Movie = movie;
        }

        public Cluster(Cluster left, Cluster right, double distance)
        {
            Distance = distance;
            Left = left;
            Right = right;
        }

        public bool IsLeaf => Movie != null;

    }
}
