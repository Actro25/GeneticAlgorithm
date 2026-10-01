using System.Collections;
namespace GeneticAlgorithm;

public interface IPhenotypesCalculation
{
    int QuantityOfChromosomes { get; set; }   
    void CalculatePhenotypes(List<Phenotype> phenotypes, out List<Phenotype[]> chunkedResult);
    public void UpdatePhenotypes(List<Phenotype> newPopulation, IFunctionGeneticAlgorithm functions, (int A, int B) distance);
}