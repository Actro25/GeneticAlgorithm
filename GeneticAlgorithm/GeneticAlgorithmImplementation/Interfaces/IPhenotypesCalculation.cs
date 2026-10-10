namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;

public interface IPhenotypesCalculation
{
    int QuantityOfChromosomes { get; set; }   
    void CalculatePhenotypes(List<Phenotype> phenotypes, out List<Phenotype[]> chunkedResult);
    public void UpdatePhenotypes(List<Phenotype> newPopulation, IFunctionGeneticAlgorithm functions, List<(double A, double B)> distance, List<List<double>> sequence, List<int> bitLengths);
}