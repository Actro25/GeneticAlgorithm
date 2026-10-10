using System.Collections;
using System.Text;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Classes.Debug;

public class GeneticAlgorithmDebug : IGeneticAlgorithmDebug
{
    public static string ToBitString(BitArray bits)
    {
        var sb = new StringBuilder(bits.Length);
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            sb.Append(bits[i] ? '1' : '0');
        }
        return sb.ToString();
    }
    
    public void PrintPhenotypes(List<Phenotype> phenotypes, string title)
    {
        if (phenotypes.Count == 0)
        {
            Console.WriteLine("There isn't any phenotypes.");
            return;
        }

        const int totalWidth = 74;

        string headerTitle = title.Length > totalWidth ? title[..(totalWidth - 3)] + "..." : title;
    
        int padding = totalWidth - headerTitle.Length;
        int padLeft = padding / 2 + headerTitle.Length;
        string centeredTitle = headerTitle.PadLeft(padLeft).PadRight(totalWidth);

        Console.ForegroundColor = ConsoleColor.Cyan;
    
        Console.WriteLine("┌───────────────────────────────────────────────────────────────────────────┐");
        Console.WriteLine($"│{centeredTitle} │");
        Console.WriteLine("├──────┬───────────────────────┬─────────────┬───────────────┬──────────────┤");
    
        Console.WriteLine("│  №   │ Chromosome (BitArray) │   Vector    │ f(x)'s Value  │ Coefficient  │");
        Console.WriteLine("├──────┼───────────────────────┼─────────────┼───────────────┬──────────────┤");
        Console.ResetColor();

        for (int i = 0; i < phenotypes.Count; i++)
        {
            var p = phenotypes[i];
            string chromoStr = p.Chromosome != null ? ToBitString(p.Chromosome) : "null";

            string vectorStr = p.Vector != null 
                ? $"[{string.Join(", ", p.Vector.Select(v => v.ToString("F4")))}]" 
                : "null";

            Console.WriteLine(
                "│ {0,-4} │ {1,-21} │ {2,11} │ {3,13:F4} │ {4,12:F4} │",
                i + 1,
                chromoStr,
                vectorStr,
                p.FunctionValue,
                p.Coefficient
            );
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└──────┴───────────────────────┴─────────────┴───────────────┴──────────────┘");
        Console.ResetColor();
    }
}