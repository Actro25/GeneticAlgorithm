using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Classes;

public class Crossover : ICrossover
{
    public int BitQuantities { get; set; }
    public ICrossoverDebug? Debug { get; set; }
    private static readonly Random _random = new Random();

    public Crossover(ICrossoverDebug? debug = null)
    {
        Debug = debug;
    }

    public void CrossoverBits(List<Phenotype> phenotypes, ref List<Phenotype[]> chunkedResult)
    {
        var logs = new List<ICrossoverDebug.CrossoverLogRecord>();
        var chunkIndex = 0;
        
        foreach (var f in chunkedResult)
        {
            //For debugging
            chunkIndex++;
            List<BitArray> oldChromosome = new List<BitArray>();
            foreach (var chromosome in f)
            {
                oldChromosome.Add(chromosome.Chromosome.Clone() as BitArray);
            }

            // If we have only one element in pair we skip it.
            if (f.Length == 1)
                continue;

            /*
             * It's a point where we start doing crossover from.
             * Random points gets from 0 to BitQuantities - 1.
             */
            var point = _random.Next(0, BitQuantities - 1);

            // Here we're doing Crossover for the point.
            for (var i = point; i < BitQuantities; i++)
            {
                (
                    f[0].Chromosome[i],
                    f[1].Chromosome[i]
                ) =
                (
                    f[1].Chromosome[i],
                    f[0].Chromosome[i]
                );
            }
            
            //For debugging
            List<BitArray> newChromosome = new List<BitArray>();
            foreach (var chromosome in f)
            {
                newChromosome.Add(chromosome.Chromosome.Clone() as BitArray);
            }
            
            logs.Add(new ICrossoverDebug.CrossoverLogRecord(
                ChunkId: chunkIndex,
                PointOfChange: point,
                OldChromosome: oldChromosome,
                NewChromosome: newChromosome
            ));
        }
        Debug?.PrintCrossoverLog(logs);
    }
}