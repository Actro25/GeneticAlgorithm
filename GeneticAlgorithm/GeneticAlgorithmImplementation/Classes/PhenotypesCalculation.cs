using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Classes;

public class PhenotypesCalculation : IPhenotypesCalculation
{
    public int QuantityOfChromosomes { get; set; }

    public PhenotypesCalculation(int quantityOfChromosomes)
    {
        QuantityOfChromosomes = quantityOfChromosomes;
    }

    public void CalculatePhenotypes(List<Phenotype> phenotypes, out List<Phenotype[]> chunkedResult)
    {
        /*
         * counts needs to save quantity of individuals that will be at the next population.
         * counts has lenght of phenotypes.Count because we need to find their next population for each individual.
         */
        var counts = new int[phenotypes.Count];
        
        // sumOfCoefficient needs for the following calculations.
        double sumOfCoefficient = phenotypes.Sum(f => f.Coefficient);

        /*
         * Here we calculate next possible population.
         * Using standard formulas for Genetic Algorithm.
         */
        for (var i = 0; i < phenotypes.Count; i++)
        {
            counts[i] = (int)Math.Round(
                phenotypes[i].Coefficient / sumOfCoefficient * QuantityOfChromosomes, 
                MidpointRounding.ToEven
            );
        }
        
        int chosenCount = counts.Sum();
        // POSSIBLE PROBLEMS WITH QuantityOfChromosomes!!!
        int remainders = QuantityOfChromosomes - chosenCount;

        /*
         * If we found that we have remainders in chosenCount,
         * we just look for the best phenotype and give it extra population.
         *
         * Remainders appears only when chosen qouantity of phenotypes don't equal to quantity of choromosome,
         * that should be at the next population.
         */
        if (remainders != 0)
        {
            var bestIndex = 0;
            for (int i = 1; i < counts.Length; i++)
            {
                if (counts[i] > counts[bestIndex])
                {
                    bestIndex = i;
                }
            }

            counts[bestIndex] += remainders;
        }
        
        // Here we create temporary List<Phenotype> for future actions.
        var newChromosomes = new List<Phenotype>(QuantityOfChromosomes);

        // Here we add phenotype in its quantity that was calculated above.
        for (var i = 0; i < phenotypes.Count; i++)
        {
            var parent = phenotypes[i];
            int quantity = counts[i];

            for (int j = 0; j < quantity; j++)
            {
                newChromosomes.Add(parent.Clone());
            }
        }

        // Here we do chunking because the following actions demand chunking.
        chunkedResult = newChromosomes
            .Chunk(2)
            .ToList();
    }
    
    public void UpdatePhenotypes(List<Phenotype> newPopulation, IFunctionGeneticAlgorithm functions, List<(double A, double B)> distance, List<List<double>> sequence, List<int> bitLengths)
    {
        for (int i = 0; i < newPopulation.Count; i++)
        {
            var phenotype = newPopulation[i];

            phenotype.Vector = phenotype.Chromosome.DecodeChromosome(sequence, bitLengths);
            phenotype.FunctionValue = functions.GetFitness(phenotype.Vector);
            phenotype.Coefficient = functions.GetCoefficient(phenotype.FunctionValue);

            newPopulation[i] = phenotype;
        }
    }
}