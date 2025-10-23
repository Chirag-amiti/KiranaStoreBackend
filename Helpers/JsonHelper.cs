using System.Text.Json;

namespace KiranaStore.Helpers
{
    public static class JsonHelper
    {
        private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataJson", "products.json");

        // Here I am reading the JSON and deserialize into a list
        public static List<T> ReadJson<T>()
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        // Here I am Serialize list to JSON and saveit to JSON file
        public static void WriteJson<T>(List<T> list)
        {
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
