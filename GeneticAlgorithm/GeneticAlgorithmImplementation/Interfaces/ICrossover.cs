using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;

public interface ICrossover
{
    int BitQuantities { get; set; }
    public ICrossoverDebug? Debug { get; set; }
    public void CrossoverBits(List<Phenotype> phenotypes, ref List<Phenotype[]> chunkedResult);
}