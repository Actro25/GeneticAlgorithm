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

        // Загальна ширина таблиці чітко зафіксована на 88 символів
        const int totalWidth = 88;

        string headerTitle = title.Length > totalWidth ? title[..(totalWidth - 3)] + "..." : title;
    
        int padding = totalWidth - headerTitle.Length;
        int padLeft = padding / 2 + headerTitle.Length;
        string centeredTitle = headerTitle.PadLeft(padLeft).PadRight(totalWidth);

        Console.ForegroundColor = ConsoleColor.Cyan;
    
        // Верхня рамка (рівно 88 символів усередині)
        Console.WriteLine("┌──────────────────────────────────────────────────────────────────────────────────────────┐");
        Console.WriteLine($"│{centeredTitle}│");
        Console.WriteLine("├──────┬───────────────────────┬──────────────────────────┬───────────────┬──────────────┤");
    
        Console.WriteLine("│  №   │ Chromosome (BitArray) │          Vector          │ f(x)'s Value  │ Coefficient  │");
        Console.WriteLine("├──────┼───────────────────────┼──────────────────────────┼───────────────┬──────────────┤");
        Console.ResetColor();

        for (int i = 0; i < phenotypes.Count; i++)
        {
            var p = phenotypes[i];
            string chromoStr = p.Chromosome != null ? ToBitString(p.Chromosome) : "null";

            string vectorStr = "null";
            if (p.Vector != null)
            {
                vectorStr = $"[{string.Join(", ", p.Vector.Select(v => v.ToString("G4")))}]";
            }

            Console.WriteLine(
                "│ {0,-4} │ {1,-21} │ {2,-24} │ {3,13:G4} │ {4,12:G4} │",
                i + 1,
                chromoStr,
                vectorStr,
                p.FunctionValue,
                p.Coefficient
            );
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        // Нижня рамка (рівно 88 символів)
        Console.WriteLine("└──────┴───────────────────────┴──────────────────────────┴───────────────┴──────────────┘");
        Console.ResetColor();
    }
}