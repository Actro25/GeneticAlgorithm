using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Classes.Debug;

public class CrossoverDebug : ICrossoverDebug
{
    private int QuantityOfChromosomeInPair { get; init; }

    public CrossoverDebug(int quantityOfChromosomeInPair)
    {
        QuantityOfChromosomeInPair = quantityOfChromosomeInPair;
    }

public void PrintCrossoverLog(List<ICrossoverDebug.CrossoverLogRecord> logs)
    {
        if (logs.Count == 0)
        {
            Console.WriteLine("No crossover operations to display.");
            return;
        }

        int chromoLength = logs[0].OldChromosome[0].Length;
        int chromoColWidth = Math.Max(chromoLength, 15);

        int totalContentWidth = 8 + 5 + (chromoColWidth + 2) + (chromoColWidth + 2) + 3;

        Console.ForegroundColor = ConsoleColor.Cyan;

        string title = "CROSSOVER LOG DETAILS (BY CHUNKS)";
        string centeredTitle = title.PadLeft((totalContentWidth + title.Length) / 2).PadRight(totalContentWidth);

        Console.WriteLine("┌" + new string('─', totalContentWidth) + "┐");
        Console.WriteLine($"│{centeredTitle}│");
        Console.WriteLine("├────────┬───────┬" + new string('─', chromoColWidth + 2) + "┬" + new string('─', chromoColWidth + 2) + "┤");

        Console.WriteLine($"│ Chunk  │ Point │ {"Old Chromosomes".PadRight(chromoColWidth)} │ {"New Chromosomes".PadRight(chromoColWidth)} │");
        Console.WriteLine("├────────┼───────┼" + new string('─', chromoColWidth + 2) + "┼" + new string('─', chromoColWidth + 2) + "┤");
        Console.ResetColor();

        foreach (var log in logs)
        {
            int phenotypesCount = log.OldChromosome.Count;

            for (int p = 0; p < phenotypesCount; p++)
            {
                if (p == 0)
                {
                    Console.Write($"│ {log.ChunkId,-6} │ {log.PointOfChange,-5} │ ");
                }
                else
                {
                    Console.Write($"│ {"",-6} │ {"",-5} │ ");
                }

                var oldChromo = log.OldChromosome[p];
                var newChromo = log.NewChromosome[p];

                PrintChromosomeWithCrossoverPoint(oldChromo, log.PointOfChange, chromoColWidth);
                Console.Write(" │ ");

                PrintChromosomeWithCrossoverPoint(newChromo, log.PointOfChange, chromoColWidth);
                Console.WriteLine(" │");
            }

            if (log != logs.Last())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("├────────┼───────┼" + new string('─', chromoColWidth + 2) + "┼" + new string('─', chromoColWidth + 2) + "┤");
                Console.ResetColor();
            }
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("└────────┴───────┴" + new string('─', chromoColWidth + 2) + "┴" + new string('─', chromoColWidth + 2) + "┘");
        Console.ResetColor();
    }

    private void PrintChromosomeWithCrossoverPoint(BitArray target, int pointOfChange, int width)
    {
        int length = target.Length;

        for (int i = length - 1; i >= 0; i--)
        {
            bool bitValue = target[i];

            if (i >= pointOfChange)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(bitValue ? '1' : '0');
                Console.ResetColor();
            }
            else
            {
                Console.Write(bitValue ? '1' : '0');
            }
        }

        if (width > length)
        {
            Console.Write(new string(' ', width - length));
        }
    }
}