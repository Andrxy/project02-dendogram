using System;
using System.IO;
using System.Text.Json;
using Project02_Dendogram.Models;
using Project02_Dendogram.Models.DataStructures;

namespace Project02_Dendogram.Services
{
    internal class JSONExporter
    {
        private readonly string _filePath = @"../../../Resources/";

        // Método que convierte el árbol de clusters a un objeto JSON.
        public ClusterJson ConvertToJson(Cluster cluster)
        {
            if (cluster == null) return null;

            ClusterJson json = new ClusterJson();

            // Distancia 
            json.d = cluster.Distance;

            // Nombre (solo hojas tienen un Movie)
            json.n = (cluster.IsLeaf) ? cluster.Movies.GetAt(0).Title : " ";

            // Hijos
            if (!cluster.IsLeaf)
            {
                json.c.Add(ConvertToJson(cluster.Left));
                json.c.Add(ConvertToJson(cluster.Right));
            }

            return json;
        }

        // Método que exporta el objeto JSON a un archivo.
        public void ExportToFile(Cluster root)
        {
            ClusterJson jsonRoot = ConvertToJson(root);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;      // indentado
            options.MaxDepth = 200;            // que sea profundo
            options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Ignorar referencias circulares

            // Convertir a JSON
            string jsonString = JsonSerializer.Serialize(jsonRoot, options);

            // la ruta del archivo
            string finalPath = Path.Combine(_filePath, "dendrogram.json");

            // escribir y guardar
            File.WriteAllText(finalPath, jsonString);
        }
    }
}
