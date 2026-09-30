namespace GeneticAlgorithm;

public interface IMutation
{
    int QuantityOfMutationsPercentage { get; set; }
    double ChanceOfMutation { get; set; }
    void MutateOneGen(ref List<Phenotype[]> chunkedResult);
}

public class Mutation : IMutation
{
    public int QuantityOfMutationsPercentage { get; set; }
    public double ChanceOfMutation { get; set; }
    private static readonly Random _random = new Random();

    public Mutation(int quantityOfMutations, double chanceOfMutation)
    {
        QuantityOfMutationsPercentage = quantityOfMutations;
        ChanceOfMutation = chanceOfMutation;
    }

    public void MutateOneGen(ref List<Phenotype[]> chunkedResult)
    {
        double perBit = QuantityOfMutationsPercentage / 100.0;
        foreach (var pair in chunkedResult)
        foreach (var p in pair)
            for (int i = 0; i < p.Chromosome.Length; i++)
                if (_random.NextDouble() < perBit)
                    p.Chromosome[i] = !p.Chromosome[i];
    }
}