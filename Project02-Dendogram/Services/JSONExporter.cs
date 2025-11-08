using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using Project02_Dendogram.Models;

namespace Project02_Dendogram.Services
{
    internal class JSONExporter
    {
        private readonly string _filePath = @"../../../Resources/";
        public ClusterJson ConvertToJson(Cluster cluster)
        {
            ClusterJson json = new ClusterJson();
            json.d = cluster.Distance;
            json.n = (cluster.Movie != null) ? cluster.Movie.Title : " ";

            if (!cluster.IsLeaf)
            {
                json.c.Add(ConvertToJson(cluster.Left));
                json.c.Add(ConvertToJson(cluster.Right));
            }

            return json;
        }

        public void ExportToFile(Cluster root)
        {
            ClusterJson jsonRoot = ConvertToJson(root);

            var options = new JsonSerializerOptions { WriteIndented = true };

            string finalPath = Path.Combine(_filePath, "dendrogram.json");

            File.WriteAllText(finalPath, JsonSerializer.Serialize(jsonRoot, options));
        }
    }
}
