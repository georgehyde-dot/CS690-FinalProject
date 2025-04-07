namespace FileSaver;
using System.IO;                
using System.Linq;              
using System.Text.Json; 

public class FileSaver
    {
        private readonly string _filename;

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        public FileSaver(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Filename cannot be null or whitespace.", nameof(filename));
            }
            this._filename = filename;

            try
            {
                string directory = Path.GetDirectoryName(this._filename);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(this._filename))
                {
                    File.WriteAllText(this._filename, "[]");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to initialize file '{this._filename}': {ex.Message}");
            }
        }

        public void SaveCollection<T>(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                collection = Enumerable.Empty<T>();
            }

            try
            {
                string jsonString = JsonSerializer.Serialize(collection, _options);
                File.WriteAllText(_filename, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to save data to '{_filename}': {ex.Message}");
            }
        }
        public List<T> LoadCollection<T>()
        {
            try
            {
                if (!File.Exists(_filename))
                {
                    Console.WriteLine($"[Warning] File not found: '{_filename}'. Returning empty list.");
                    return new List<T>();
                }

                string jsonString = File.ReadAllText(_filename);

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return new List<T>(); 
                }

                List<T> loadedData = JsonSerializer.Deserialize<List<T>>(jsonString, _options);
                return loadedData ?? new List<T>(); 
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"[Error] Invalid JSON format in '{_filename}': {jsonEx.Message}. Returning empty list.");
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to load data from '{_filename}': {ex.Message}. Returning empty list.");
                return new List<T>();
            }
        }
    }

