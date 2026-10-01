using System.Collections;

namespace GeneticAlgorithm.Classes;

public class PhenotypesCalculation : IPhenotypesCalculation
{
    public int QuantityOfChromosomes { get; set; }

    public PhenotypesCalculation(int quantityOfChromosomes)
    {
        QuantityOfChromosomes = quantityOfChromosomes;
    }

    public void CalculatePhenotypes(List<Phenotype> phenotypes, out List<Phenotype[]> chunkedResult)
    {
        //Creating new temporary fenotypes.
        var fenotypeList = new List<(double Value, int QuantityInNewArray)>();
        
        //Sum of all cofficients for formula.
        double sumOfCoefficient = phenotypes.Sum(f => f.Coefficient);

        for (int i = 0; i < phenotypes.Count; i++)
        {
            //Calculate quantity of new Fenotype for next generation.
            int quantity = (int)Math.Round(
                phenotypes[i].Coefficient / sumOfCoefficient * QuantityOfChromosomes, 
                MidpointRounding.ToEven
            );

            fenotypeList.Add((phenotypes[i].Value, quantity));
        }
        
        //Getting sum of all chosen elements to the next generation.
        int chosenCount = fenotypeList.Sum(f => f.QuantityInNewArray);
        //Getting difference.
        int remainders = QuantityOfChromosomes - chosenCount;

        //If difference not equal to 0, it means that we're lacking some elements that is written in remainders.
        //So we have to add lacked elements.
        if (remainders != 0)
        {
            // This is the easiest and the speedest way of finding the max quantity of fenotipe.
            int bestIndex = 0;
            for (int i = 1; i < fenotypeList.Count; i++)
            {
                if (fenotypeList[i].QuantityInNewArray > fenotypeList[bestIndex].QuantityInNewArray)
                {
                    bestIndex = i;
                }
            }

            var best = fenotypeList[bestIndex];
            //When we found the best element we add remainders to his QualityInNewArray.
            fenotypeList[bestIndex] = (best.Value, best.QuantityInNewArray + remainders);
        }
        
        //Creating new list for result.
        var newChromosomes = new List<Phenotype>();

        //Here we are creating a new chosen generation that will later be modified.
        for (int i = 0; i < fenotypeList.Count; i++)
        {
            //Here we're looking for fenotype that have equal value. If value equal it means that other fields are equal too.
            var selected = phenotypes.First(f => f.Value == fenotypeList[i].Value);
            int j = 0;
            while (j < fenotypeList[i].QuantityInNewArray)
            {
                //Adding selected element N times.
                newChromosomes.Add(new Phenotype
                {
                    Value = selected.Value,

                    Chromosome =
                        new BitArray(selected.Chromosome),

                    FunctionValue =
                        selected.FunctionValue,

                    Coefficient =
                        selected.Coefficient,
                });
                j++;
            }
        }
        
        //Chunking element into pairs.
        chunkedResult = newChromosomes
            .Chunk(2)
            .ToList();
    }
    public void UpdatePhenotypes(List<Phenotype> newPopulation, IFunctionGeneticAlgorithm functions, (int A, int B) distance)
    {
        for (int i = 0; i < newPopulation.Count; i++)
        {
            int value = BitArrayToInt(newPopulation[i].Chromosome, distance);
            double fitness = functions.GetFitness(value * functions.Precision);
            double coefficient = functions.GetCoefficient(fitness);

            newPopulation[i] = new Phenotype
            {
                Value = value,
                FunctionValue = fitness,
                Coefficient = coefficient,
                Chromosome = newPopulation[i].Chromosome
            };
        }
    }
    private int BitArrayToInt(BitArray bitArray, (int A, int B) distance)
    {
        int offset = 0;

        for (int i = 0; i < bitArray.Count; i++)
        {
            if (bitArray[i])
            {
                offset |= (1 << i);
            }
        }

        int value = distance.A + offset;

        if (value < distance.A) value = distance.A;
        if (value > distance.B) value = distance.B;

        return value;
    }
}