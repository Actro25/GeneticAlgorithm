namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

public interface IMutation
{
    double QuantityOfMutationsPercentage { get; set; }
    double ChanceOfMutation { get; set; }
    IMutationDebug? Debug { get; set; }
    void MutateOneGen(ref List<Phenotype[]> chunkedResult);
}