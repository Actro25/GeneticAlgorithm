namespace GeneticAlgorithm;

public interface IMutation
{
    double QuantityOfMutationsPercentage { get; set; }
    double ChanceOfMutation { get; set; }
    void MutateOneGen(ref List<Phenotype[]> chunkedResult);
}