using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

internal class Cluster
{
    public CustomList<Movie> Movies { get; set; }
    public CustomList<int> Indexes { get; set; }
    public Cluster Left { get; set; }
    public Cluster Right { get; set; }
    public double Distance { get; set; }

    public bool IsLeaf => Left == null && Right == null;

    public Cluster(Movie movie, int index)
    {
        Movies = new CustomList<Movie>();
        Movies.Add(movie);

        Indexes = new CustomList<int>();
        Indexes.Add(index);

        Left = null;
        Right = null;
    }

    public Cluster(Cluster A, Cluster B, double distance)
    {
        Left = A;
        Right = B;
        Distance = distance;

        Movies = new CustomList<Movie>();
        Movies.Copy(A.Movies);
        Movies.Copy(B.Movies);

        Indexes = new CustomList<int>();
        Indexes.Copy(A.Indexes);
        Indexes.Copy(B.Indexes);
    }
}
