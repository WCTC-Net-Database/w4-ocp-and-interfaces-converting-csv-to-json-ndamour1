using W4_assignment_template.Interfaces;
using W4_assignment_template.Models;

namespace W4_assignment_template.Services;

public class JsonFileHandler : IFileHandler
{
    private string jsonPath = "Files/input.json";

    public JsonFileHandler(string filePath)
    {
        jsonPath = filePath ?? throw new ArgumentNullException(nameof(filePath)); // Ensure file path is not null
    }

    // Method to read characters from a JSON file
    public List<Character> ReadCharactersFromFile(string filePath)
    {
        var json = File.ReadAllText(jsonPath);
        var characters = JsonSerializer.Deserialize<List<Character>>(json);

        if (!File.Exists(jsonPath))
        {
            Console.Write("File not found.");
            return characters;
        }

        // Method to parse JSON lines
        string[] ParseJsonLine(string json)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var currentPart = new System.Text.StringBuilder();

            foreach (char c in json)
            {
                if (c == '\"')
                {
                    inQuotes = !inQuotes; // Toggle whether we are inside quotes
                }
                else if (c == ',' && !inQuotes)
                {
                    // If we hit a comma outside of quotes, it's a delimiter
                    result.Add(currentPart.ToString().Trim());
                    currentPart.Clear();
                }
                else
                {
                    // Append to the current field (inside quotes or not)
                    currentPart.Append(c);
                }
            }

            // Add the final part
            result.Add(currentPart.ToString().Trim());
            return result.ToArray();
        }

        foreach (var line in json.Skip(1)) // Skip header
        {
            var parts = ParseJsonLine(json);
            if (parts.Length == 5)
            {
                characters.Add(new Character
                {
                    name = parts[0],
                    characterClass = parts[1],
                    level = int.Parse(parts[2]),
                    hitPoints = int.Parse(parts[3]),
                    equipment = parts[4].Split('|')
                });
            }
        }

        return characters;
    }

    // Method to find character by name using LINQ
    public Character FindCharactersByName(string name)
    {
        var characters = ReadCharactersFromFile(jsonPath);
        var chosen = characters.FirstOrDefault<Character>(c => c.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (chosen != null)
        {
            return chosen;
        }
        else
        {
            Console.WriteLine("Character not found.\n");
            return null;
        }
    }

    // Method to write characters to a JSON file
    public void WriteCharactersToFile(string filePath, List<Character> characters)
    {
        var json = JsonSerializer.Serialize<List<Character>>(characters, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json);
    }
}
