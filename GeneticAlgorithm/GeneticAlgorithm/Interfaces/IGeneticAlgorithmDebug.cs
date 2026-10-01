using System.Collections;
using System.Text;

namespace GeneticAlgorithm.Interfaces;

public interface IGeneticAlgorithmDebug
{
    void PrintPhenotypes(List<Phenotype> phenotypes, string title);
}