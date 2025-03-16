using System.Diagnostics;

int firstNumber = 0;
int secondNumber = 1;
string separator = "######################";
bool isInRandomGame = false;
List<string> pastGames = new List<string>() { separator };
Random randomNumberGenerator = new Random();
var operations = new Dictionary<string, Func<int, int, int>>
{
    { "+", (a, b) => a + b },
    { "-", (a, b) => a - b },
    { "*", (a, b) => a * b },
    { "/", (a, b) => b != 0 ? a / b : throw new DivideByZeroException("Cannot divide by zero!") }
};

while (true)
{
    ShowMenu();
    isInRandomGame = false;
    string input = Console.ReadLine();

    string validatedInput = IsValidInput(["+", "-", "*", "/", "r", "R", "p", "P", "q", "Q"], input, "Incorrect input: Please input a valid arithmetic operator or press 'q' to quit the program");

    if (validatedInput == "q" || validatedInput == "Q")
    {
        Console.WriteLine("Goodbye!");
        Environment.Exit(0);
    }

    if (validatedInput == "p" || validatedInput == "P")
    {
        pastGames.ForEach(Console.WriteLine);
    }
    else
    {
        Operation(validatedInput);
    }
}

void ShowMenu()
{
    Console.WriteLine("Please enter an operator, press r for random operations mode, p for past games or q to quit the program:");
    Console.WriteLine("Addition (+)");
    Console.WriteLine("Subtraction (-)");
    Console.WriteLine("Multiplication (*)");
    Console.WriteLine("Division (/)");
    Console.WriteLine("Random operations mode (r)");
    Console.WriteLine("Past Games (p)");
    Console.WriteLine("Quit (q)");
}

string SetDifficulty()
{
    Console.WriteLine($"Please choose difficulty:");
    Console.WriteLine($"1. Easy");
    Console.WriteLine($"2. Medium");
    Console.WriteLine($"3. Hard");

    return IsValidInput(["1", "2", "3"], Console.ReadLine(), "Incorrent Input: Please choose a difficulty: Easy (1), Medium (2) or Hard (3)");
}

void SetNumbers(string operation, int maxNumber, int maxDivisionNumber)
{
    if (operation != "/")
    {
        firstNumber = randomNumberGenerator.Next(maxNumber);
        secondNumber = randomNumberGenerator.Next(maxNumber);
    }
    else
    {
        firstNumber = randomNumberGenerator.Next(maxDivisionNumber);
        secondNumber = randomNumberGenerator.Next(1, maxDivisionNumber);
        while (firstNumber % secondNumber != 0)
        {
            secondNumber = randomNumberGenerator.Next(1, maxDivisionNumber);
        }
    }
}

int ValidateIsNumber(string input)
{
    bool isInputNumeric = int.TryParse(input, out int result);
    while (!isInputNumeric)
    {
        Console.WriteLine("Please input a number!");
        input = Console.ReadLine();
        isInputNumeric = int.TryParse(input, out result);
    }

    return result;
}

string IsValidInput(string[] options, string input, string message = "Incorrect Input: Please try again.")
{
    while (!options.Contains(input))
    {
        Console.WriteLine(message);
        input = Console.ReadLine();
    }

    return input;
}

void ExecuteOperation(string operation)
{

    if (operation == "r" || operation == "R")
    {
        isInRandomGame = true;
        operation = operations.Keys.OrderBy(op => Guid.NewGuid()).First();
    }

    string difficulty = SetDifficulty();

    switch (difficulty)
    {
        case "1":
            SetNumbers(operation, 101, 11);
            break;
        case "2":
            SetNumbers(operation, 501, 51);
            break;
        case "3":
            SetNumbers(operation, 1001, 101);
            break;
    }

    Console.WriteLine($"{firstNumber} {operation} {secondNumber} = ?");
    pastGames.Add($"Game: {firstNumber} {operation} {secondNumber} = ?");

    Stopwatch stopwatch = Stopwatch.StartNew();

    var validatedInputnumber = ValidateIsNumber(Console.ReadLine());

    bool result = false;
    if (operations.TryGetValue(operation, out var function))
    {
        result = validatedInputnumber == function(firstNumber, secondNumber);
    }

    while (!result)
    {
        pastGames.Add($"Attempt: {validatedInputnumber}");

        Console.WriteLine($"Sorry, that is wrong. You have spent {stopwatch.Elapsed.TotalSeconds:F2} seconds trying to figure out this challenge.");
        Console.WriteLine("Would you like to try again, get another challenge or move back to the menu?");
        Console.WriteLine("1. Try again!");
        Console.WriteLine("2. New challenge");
        Console.WriteLine("3. Move back to menu");

        string validatedInput = IsValidInput(["1", "2", "3"], Console.ReadLine(), "Incorrent Input: Would you like to try again (0), get a new challenge (1) or move back to the menu? (2)");

        if (validatedInput == "1")
        {
            Console.WriteLine($"{firstNumber} {operation} {secondNumber} = ?");

            validatedInputnumber = ValidateIsNumber(Console.ReadLine());
            result = validatedInputnumber == function(firstNumber, secondNumber);
        }
        else if (validatedInput == "2")
        {
            pastGames.Add($"Time spent with challenge: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            pastGames.Add(separator);

            if (isInRandomGame)
                Operation("r");
            else
                Operation(operation);
            break;
        }
        else
        {
            pastGames.Add($"Time spent with challenge: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
            pastGames.Add(separator);
            break;
        }
    }

    if (result)
    {
        stopwatch.Stop();

        pastGames.Add($"Solution: {validatedInputnumber}");
        pastGames.Add($"Time took to complete: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        pastGames.Add(separator);

        Console.WriteLine($"Congratulations, you win! It took you {stopwatch.Elapsed.TotalSeconds:F2} seconds to complete this challenge. Would you like another challenge or move back to the menu?");
        Console.WriteLine("1. New challenge");
        Console.WriteLine("2. Move back to menu");

        string validatedInput = IsValidInput(["1", "2"], Console.ReadLine(), "Incorrect input: Would you like a new challenge (1), or move back to the menu? (2)");

        if (validatedInput == "1")
        {
            if (isInRandomGame)
                Operation("r");
            else
                Operation(operation);
        }
    }
}
