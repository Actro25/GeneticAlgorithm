using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Classes;

public class MutationDebug : IMutationDebug
{
    public void PrintMutationTable(List<IMutationDebug.MutationLogRecord> logs)
    {
        if (logs == null || logs.Count == 0)
        {
            Console.WriteLine("No mutations occurred.");
            return;
        }

        int chromoLength = logs[0].OldChromosome.Length;
        int chromoColWidth = Math.Max(chromoLength, 15); 

        int totalContentWidth = 6 + (chromoColWidth + 2) + (chromoColWidth + 2) + 2;

        Console.ForegroundColor = ConsoleColor.Cyan;
        
        string title = "MUTATION LOG DETAILS";
        string centeredTitle = title.PadLeft((totalContentWidth + title.Length) / 2).PadRight(totalContentWidth);

        Console.WriteLine("┌" + new string('─', totalContentWidth) + "┐");
        Console.WriteLine($"│{centeredTitle}│");
        Console.WriteLine("├──────┬" + new string('─', chromoColWidth + 2) + "┬" + new string('─', chromoColWidth + 2) + "┤");

        Console.WriteLine($"│  №   │ {"Old Chromosome".PadRight(chromoColWidth)} │ {"New Chromosome".PadRight(chromoColWidth)} │");
        Console.WriteLine("├──────┼" + new string('─', chromoColWidth + 2) + "┼" + new string('─', chromoColWidth + 2) + "┤");
        Console.ResetColor();

        foreach (var log in logs)
        {
            Console.Write($"│ {log.PhenotypeId,-4} │ ");

            PrintHighlightedChromosome(log.OldChromosome, log.NewChromosome, chromoColWidth);
            Console.Write(" │ ");

            PrintHighlightedChromosome(log.NewChromosome, log.OldChromosome, chromoColWidth);
            Console.WriteLine(" │");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└──────┴" + new string('─', chromoColWidth + 2) + "┴" + new string('─', chromoColWidth + 2) + "┘");
        Console.ResetColor();
    }

    private void PrintHighlightedChromosome(BitArray target, BitArray reference, int width)
    {
        int length = target.Length;

        for (int i = length - 1; i >= 0; i--)
        {
            bool targetBit = target[i];
            bool refBit = reference[i];

            if (targetBit != refBit)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(targetBit ? '1' : '0');
                Console.ResetColor();
            }
            else
            {
                Console.Write(targetBit ? '1' : '0');
            }
        }

        if (width > length)
        {
            Console.Write(new string(' ', width - length));
        }
    }
}