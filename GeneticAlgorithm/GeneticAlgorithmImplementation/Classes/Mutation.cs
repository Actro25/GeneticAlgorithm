using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Classes;

public class Mutation : IMutation
{
    public double QuantityOfMutationsPercentage { get; set; }
    public double ChanceOfMutation { get; set; }
    public IMutationDebug? Debug { get; set; }
    private readonly Random _random = new Random();

    public Mutation(double quantityOfMutations, double chanceOfMutation, IMutationDebug? debug = null)
    {
        QuantityOfMutationsPercentage = quantityOfMutations;
        ChanceOfMutation = chanceOfMutation;
        Debug = debug;
    }

    public void MutateOneGen(ref List<Phenotype[]> chunkedResult)
    {
        if (_random.NextDouble() > ChanceOfMutation)
            return;
        
        //Getting quantity of loops
        var quantityOfBits = chunkedResult.First()[0].Chromosome.Length;
        var quantityOfPhenotypesInChunk = chunkedResult.First().Length;
        var quantityOfPairs = chunkedResult.Count;
        
        var quantityOfAllBits = quantityOfBits * quantityOfPhenotypesInChunk * quantityOfPairs;
        
        var quantityOfLoops = (int)Math.Round(quantityOfAllBits * QuantityOfMutationsPercentage, MidpointRounding.ToEven);

        //Calculation every random mutation in loop
        
        //For debugging
        var logs = new List<IMutationDebug.MutationLogRecord>();
        
        for (var i = 0; i < quantityOfLoops; i++)
        {
            var chosenPair = _random.Next(quantityOfPairs);
            var chosenPhenotypeInChunk = _random.Next(quantityOfPhenotypesInChunk);
            var chosenBits = _random.Next(quantityOfBits);
            
            var targetChromosome = chunkedResult[chosenPair][chosenPhenotypeInChunk].Chromosome;
            
            //For debugging
            var oldChromosome = targetChromosome.Clone();
            
            targetChromosome[chosenBits] = !targetChromosome[chosenBits];

            //For debugging
            if (Debug != null)
            {
                logs.Add(new IMutationDebug.MutationLogRecord(
                    PhenotypeId: (chosenPair * quantityOfPhenotypesInChunk) + chosenPhenotypeInChunk + 1,
                    OldChromosome: (BitArray)oldChromosome,
                    NewChromosome: targetChromosome
                ));
            }
        }
        
        //For debugging
        Debug?.PrintMutationLog(logs);
    }
}