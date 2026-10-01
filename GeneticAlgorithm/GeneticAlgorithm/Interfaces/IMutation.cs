namespace GeneticAlgorithm;

public interface IMutation
{
    int QuantityOfMutationsPercentage { get; set; }
    double ChanceOfMutation { get; set; }
    void MutateOneGen(ref List<Phenotype[]> chunkedResult);
}