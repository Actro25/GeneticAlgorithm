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
        foreach (var pair in chunkedResult)
        foreach (var p in pair)
            for (int i = 0; i < p.Chromosome.Length; i++)
                if (_random.NextDouble() < QuantityOfMutationsPercentage)
                    p.Chromosome[i] = !p.Chromosome[i];
    }
}