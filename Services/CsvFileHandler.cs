using W4_assignment_template.Interfaces;
using W4_assignment_template.Models;

namespace W4_assignment_template.Services;

public class CsvFileHandler : IFileHandler
{
    private string csvPath = "Files/input.csv";

    public CsvFileHandler(string filePath)
    {
        csvPath = filePath ?? throw new ArgumentNullException(nameof(filePath)); // Ensure file path is not null
    }

    // Method to read characters from file
    public List<Character> ReadCharactersFromFile(string filePath)
    {
        var characters = new List<Character>();

        if (!File.Exists(csvPath))
        {
            Console.Write("File not found.");
            return characters;
        }

        var lines = File.ReadAllLines(csvPath);

        // Method to parse CSV lines
        string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var currentPart = new System.Text.StringBuilder();

            foreach (char c in line)
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

        foreach (var line in lines.Skip(1)) // Skip header
        {
            var parts = ParseCsvLine(line);
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
        var characters = ReadCharactersFromFile(csvPath);
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

    public void WriteCharactersToFile(string filePath, List<Character> characters)
    {
        if (characters == null) throw new ArgumentNullException(nameof(characters)); // Ensure character list is not null

        var lines = new List<string> { "Name,Class,Level,HP,Equipment" }; // Fix header spacing
        lines.AddRange(characters.Select(c =>
        {
            // Handle names with commas by re-quoting them
            string formattedName = c.name.Contains(",") ? $"\"{c.name}\"" : c.name;

            // Join equipment with '|' and ensure it handles null value
            string formattedEquipment = string.Join("|", c.equipment ?? new string[0]);

            // Return properly formatted line
            return $"{formattedName},{c.characterClass},{c.level},{c.hitPoints},{formattedEquipment}";
        }));
        try
        {
            File.WriteAllLines(csvPath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
        }
    }
}
