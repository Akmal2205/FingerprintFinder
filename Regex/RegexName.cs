using System.Text.RegularExpressions;

public class RegexName
{
    // only for testing
    public static void Main(string[] args)
    {
        Console.Write("Masukkan nama corrupt: ");
        string sentence = Console.ReadLine();
        sentence = RegexName.FixCorruptedName(sentence);
        Console.Write("Hasil perbaikan nama corrupt: ");
        Console.WriteLine(sentence);
    }

    // Method for fixing corrupted name
    public static string FixCorruptedName(string corruptedName)
    {
        return FixUpperLower(FixNumber(corruptedName));
    }

    private static string FixNumber(string corruptedName)
    {
        // Regex pattern to match number
        string pattern = "[01234567]";

        // Transform each match
        string result = Regex.Replace(corruptedName, pattern, NumberToChar);

        return result;
    }

    private static string NumberToChar(Match number)
    {
        switch (number.Value)
        {
            case "0":
                return "o";
            case "1":
                return "i";
            case "2":
                return "z";
            case "3":
                return "e";
            case "4":
                return "a";
            case "5":
                return "s";
            case "6":
                return "g";
            case "7":
                return "t";
            default:
                return number.Value;
        }
    }

    private static string FixUpperLower(string input)
    {
        // Regex pattern to match each word
        string pattern = @"\w+";

        // Transform each match
        string result = Regex.Replace(input, pattern, new MatchEvaluator(CapitalizeWord));

        return result;
    }

    private static string CapitalizeWord(Match word)
    {
        string wordValue = word.Value;
        if (wordValue.Length > 1)
        {
            return char.ToUpper(wordValue[0]) + wordValue.Substring(1).ToLower();
        }
        else
        {
            return wordValue.ToUpper();
        }
    }
}
