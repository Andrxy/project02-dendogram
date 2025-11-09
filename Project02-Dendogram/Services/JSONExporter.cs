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

            // Distance del merge
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

            // Crear las opciones de serialización
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;      // Formatear el JSON con sangrías
            options.MaxDepth = 200;            // Establecer un límite de profundidad mayor, ajusta según sea necesario
            options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Ignorar referencias circulares

            // Convertir a JSON usando las opciones configuradas
            string jsonString = JsonSerializer.Serialize(jsonRoot, options);

            // Generar la ruta completa del archivo de salida
            string finalPath = Path.Combine(_filePath, "dendrogram.json");

            // Escribir el JSON en el archivo
            File.WriteAllText(finalPath, jsonString);
        }
    }
}
