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
    
    public void MutateOneGen(ref List<Phenotype[]> chunkedResult)
    {
        if (chunkedResult.Count == 0)
            return;

        if (_random.NextDouble() < ChanceOfMutation)
        {
            var quantityOfMutations = (int)Math.Round(
                (
                    chunkedResult.Count *
                    chunkedResult.First().Length *
                    chunkedResult.First().First().Chromosome.Count
                ) / (double)QuantityOfMutationsPercentage,
                MidpointRounding.ToEven);

            for (int i = 0; i < quantityOfMutations; i++)
            {
                var randomGroup =
                    _random.Next(0, chunkedResult.Count);

                var randomChromosome =
                    _random.Next(
                        0,
                        chunkedResult[randomGroup].Length);

                var randomGen =
                    _random.Next(
                        0,
                        chunkedResult[randomGroup]
                            [randomChromosome]
                            .Chromosome
                            .Length);

                chunkedResult[randomGroup]
                        [randomChromosome]
                        .Chromosome[randomGen] =
                    !chunkedResult[randomGroup]
                        [randomChromosome]
                        .Chromosome[randomGen];
            }
        }
    }
}