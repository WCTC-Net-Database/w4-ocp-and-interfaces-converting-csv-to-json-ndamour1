using W4_assignment_template.Interfaces;
using W4_assignment_template.Models;
using CharacterConsole;

namespace W4_assignment_template.Services;

public class CharacterManager
{
    private readonly IInput _input;
    private readonly IOutput _output;
    private string _filePath = "input.json";

    private string[] lines;

    public CharacterManager(IInput input, IOutput output)
    {
        _input = input;
        _output = output;
    }

    public void Run()
    {
        _output.WriteLine("Welcome to Character Management");

        lines = File.ReadAllLines(_filePath);

        while (true)
        {
            // Main menu
            Console.Clear();
            _output.Write("\nSelect what you want to do.\n1. Display Characters\n2. Find Character\n3. Add Character\n4. Level Up Character\n5. Exit\n");

            if (!int.TryParse(_input.ReadLine(), out int choice))
            {
                _output.Write("Invalid input. Please enter a number. ");
                continue;
            }

            switch (choice)
            {
                case 1:
                    DisplayCharacters();
                    break;
                case 2:
                    FindCharacter();
                    break;
                case 3:
                    AddCharacter();
                    break;
                case 4:
                    LevelUpCharacter();
                    break;
                case 5:
                    _output.Write("See you again!");
                    return;
                default:
                    _output.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            // Pause to allow the user to see the result before the menu is shown again
            _output.Write("Press any key to continue...");
            _input.ReadLine();
        }
    }

    // Method to display characters
    public void DisplayCharacters()
    {
        if (_filePath.Equals("input.csv"))
        {
            CsvFileHandler fileHandler = new CsvFileHandler(_filePath);
            var characters = fileHandler.ReadCharactersFromFile(_filePath);

            if (characters.Count == 0)
            {
                _output.WriteLine("No characters found.");
            }
            else
            {
                foreach (var character in characters)
                {
                    Console.WriteLine(character);
                }
            }
        }
    }

    // Method to find character
    public void FindCharacter()
    {
        if (_filePath.Equals("input.csv"))
        {
            CsvFileHandler findCharacters = new CsvFileHandler(_filePath);
            _output.Write("Enter the character's name: ");
            string name = _input.ReadLine();
            findCharacters.FindCharactersByName(name);
        }
        else if (_filePath.Equals("input.json"))
        {
            JsonFileHandler findCharacters = new JsonFileHandler(_filePath);
            _output.Write("Enter the character's name: ");
            string name = _input.ReadLine();
            findCharacters.FindCharactersByName(name);
        }
    }

    // Method to add characters
    public void AddCharacter()
    {
        // variable to break while loop below
        bool notZero = false;

        // Input for character's name
        _output.Write("Enter your character's first name: ");
        string name = _input.ReadLine();
        if (name.StartsWith("\"") && name.EndsWith("\""))
        {
            name = name.Substring(1, name.Length - 1);
        }

        // Input for character's class
        _output.Write("Enter your character's class: ");
        string characterClass = _input.ReadLine();

        // While loop to make sure level is greater than 0
        int level = 0;
        while (!notZero)
        {
            _output.Write("Enter your character's level. It must be 1 or higher. ");
            level = int.Parse(_input.ReadLine());

            if (level <= 0)
            {
                _output.Write("The number you entered is less than 1. Try again. ");
                level = int.Parse(_input.ReadLine());
            }
            else
            {
                notZero = true;
            }
        }

        // Calculation for hit points
        int hitPoints = level * 6;

        // Input for character's equipment
        _output.Write("Enter your character's equipment (separate items with a '|'): ");
        string[] equipment = _input.ReadLine().Split('|');

        // Displays the user's input for the character
        if (_filePath.Equals("input.csv"))
        {
            // CharacterReader call
            CsvFileHandler characterList = new CsvFileHandler(_filePath);
            var characters = characterList.ReadCharactersFromFile(_filePath);
            characters.Add(new Character { name = name, characterClass = characterClass, level = level, hitPoints = hitPoints, equipment = equipment });

            // CharacterWriter call
            CsvFileHandler newList = new CsvFileHandler(_filePath);
            newList.WriteCharactersToFile(_filePath, characters);
            _output.WriteLine($"Welcome, {name} the {characterClass}! You are level {level} with {hitPoints} HP and your equipment includes: {string.Join(", ", equipment)}.");
        }
        else if (_filePath.Equals("input.json"))
        {
            // CharacterReader call
            JsonFileHandler characterList = new JsonFileHandler(_filePath);
            var characters = characterList.ReadCharactersFromFile(_filePath);
            characters.Add(new Character { name = name, characterClass = characterClass, level = level, hitPoints = hitPoints, equipment = equipment });

            // CharacterWriter call
            JsonFileHandler newList = new JsonFileHandler(_filePath);
            newList.WriteCharactersToFile(_filePath, characters);
            _output.WriteLine($"Welcome, {name} the {characterClass}! You are level {level} with {hitPoints} HP and your equipment includes: {string.Join(", ", equipment)}.");
        }
    }

    // Method for leveling up character
    public void LevelUpCharacter()
    {
        _output.Write("Enter the number indexed to the character you want to level up.\n");

        if (_filePath.Equals("input.csv"))
        {
            CsvFileHandler characterList = new CsvFileHandler(_filePath);
            var characters = characterList.ReadCharactersFromFile(_filePath);
            for (int i = 0; i < characters.Count; i++)
            {
                _output.WriteLine($"{i + 1}. {characters[i].name} the {characters[i].characterClass}, Level {characters[i].level}");
            }
            int listNumber = int.Parse(_input.ReadLine()) - 1;
            Character chosen = characters[listNumber];
            int currLevel = chosen.level;
            int newLevel = 0;

            // Loop to make sure user inputs a number greater than chosen character's current level
            while (newLevel <= currLevel)
            {
                _output.Write($"You have chosen {chosen.name}.\nEnter your character's new level. It must be higher than their current level. ");
                newLevel = int.Parse(_input.ReadLine());

                if (newLevel > currLevel)
                {
                    _output.Write($"{chosen.name} is now level {newLevel} with {newLevel * 6} HP.\n");
                    characters[listNumber].level = newLevel;
                    characters[listNumber].hitPoints = newLevel * 6;
                    CsvFileHandler newList = new CsvFileHandler(_filePath);
                    newList.WriteCharactersToFile(_filePath, characters);
                }
                else if (newLevel < chosen.level)
                {
                    _output.Write($"The number you typed is less than {currLevel}. Try again. ");
                    newLevel = int.Parse(_input.ReadLine());
                }
                else if (newLevel == chosen.level)
                {
                    _output.Write($"{newLevel} is {chosen.name}'s current level. Try again. ");
                    newLevel = int.Parse(_input.ReadLine());
                }
            }
        }
        else if (_filePath.Equals("input.json"))
        {
            JsonFileHandler characterList = new JsonFileHandler(_filePath);
            var characters = characterList.ReadCharactersFromFile(_filePath);
            for (int i = 0; i < characters.Count; i++)
            {
                _output.WriteLine($"{i + 1}. {characters[i].name} the {characters[i].characterClass}, Level {characters[i].level}");
            }
            int listNumber = int.Parse(_input.ReadLine()) - 1;
            Character chosen = characters[listNumber];
            int currLevel = chosen.level;
            int newLevel = 0;

            // Loop to make sure user inputs a number greater than chosen character's current level
            while (newLevel <= currLevel)
            {
                _output.Write($"You have chosen {chosen.name}.\nEnter your character's new level. It must be higher than their current level. ");
                newLevel = int.Parse(_input.ReadLine());

                if (newLevel > currLevel)
                {
                    _output.Write($"{chosen.name} is now level {newLevel} with {newLevel * 6} HP.\n");
                    characters[listNumber].level = newLevel;
                    characters[listNumber].hitPoints = newLevel * 6;
                    CsvFileHandler newList = new CsvFileHandler(_filePath);
                    newList.WriteCharactersToFile(_filePath, characters);
                }
                else if (newLevel < chosen.level)
                {
                    _output.Write($"The number you typed is less than {currLevel}. Try again. ");
                    newLevel = int.Parse(_input.ReadLine());
                }
                else if (newLevel == chosen.level)
                {
                    _output.Write($"{newLevel} is {chosen.name}'s current level. Try again. ");
                    newLevel = int.Parse(_input.ReadLine());
                }
            }
        }
    }
}