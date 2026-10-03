namespace GeneticAlgorithm.Classes;

public class Mutation : IMutation
{
    public double QuantityOfMutationsPercentage { get; set; }
    public double ChanceOfMutation { get; set; }
    private static readonly Random _random = new Random();

    public Mutation(double quantityOfMutations, double chanceOfMutation)
    {
        QuantityOfMutationsPercentage = quantityOfMutations;
        ChanceOfMutation = chanceOfMutation;
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
        for (var i = 0; i < quantityOfLoops; i++)
        {
            var chosenPair = _random.Next(quantityOfPairs);
            var chosenPhenotypeInChunk = _random.Next(quantityOfPhenotypesInChunk);
            var chosenBits = _random.Next(quantityOfBits);
            
            chunkedResult[chosenPair][chosenPhenotypeInChunk].Chromosome[chosenBits] = !chunkedResult[chosenPair][chosenPhenotypeInChunk].Chromosome[chosenBits];
        }
    }
}