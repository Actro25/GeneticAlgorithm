namespace GeneticAlgorithm;

public interface ICrossover
{
    int BitQuantities { get; set; }
    public void CrossoverWithOnePoint(List<Phenotype> phenotypes, ref List<Phenotype[]> chunkedResult);
}