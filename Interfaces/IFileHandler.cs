using W4_assignment_template.Models;

namespace W4_assignment_template.Interfaces;

public interface IFileHandler
{
    List<Character> ReadCharactersFromFile(string filePath);
    void WriteCharactersToFile(string filePath, List<Character> characters);
}
