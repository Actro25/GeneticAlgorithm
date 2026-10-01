using System.Collections;
using System.Text;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Classes;

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
    
    public void PrintPhenotypes(List<Phenotype> phenotypes)
    {
        if (phenotypes == null || phenotypes.Count == 0)
        {
            Console.WriteLine("Список фенотипів порожній.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("┌──────┬──────────────────────┬─────────────┬───────────────┬─────────────┐");
        Console.WriteLine("│  №   │ Хромосома (BitArray) │ Значення X  │ Значення f(x) │ Коефіцієнт  │");
        Console.WriteLine("├──────┼──────────────────────┼─────────────┼───────────────┼─────────────┤");
        Console.ResetColor();

        for (int i = 0; i < phenotypes.Count; i++)
        {
            var p = phenotypes[i];
            string chromoStr = p.Chromosome != null ? ToBitString(p.Chromosome) : "null";

            Console.WriteLine(
                "│ {0,-4} │ {1,-20} │ {2,11:F4} │ {3,13:F4} │ {4,11:F4} │",
                i + 1,
                chromoStr,
                p.Value,
                p.FunctionValue,
                p.Coefficient
            );
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└──────┴──────────────────────┴─────────────┴───────────────┴─────────────┘");
        Console.ResetColor();
    }
}