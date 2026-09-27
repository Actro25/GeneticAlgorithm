using System.Collections;

namespace GeneticAlgorithm1;

public struct Fenotype
{
    public int Value;
    public BitArray Chromosome;
    public double FunctionValue;
    public double Coefficient;
    public double ExpectedQuantityAtFathersArray;
    public int ChosenQuantityAtFathersArray;
}

public class GeneticAlgorithm
{
    public int A { get; set; }
    public int B { get; set; }
    public int PopulationPercentage { get; set; }
    public int MutationPercentage { get; set; }
    public double ChanceOfMutation { get; set; }
    public int BitQuantities { get; set; }
    public int QuantityOfChromosomes { get; set; }

    private static readonly Random _random = new Random();

    public List<Fenotype> Fenotypes = new List<Fenotype>();
    public List<Fenotype> InitialPopulation = new List<Fenotype>();

    private Fenotype _bestFenotype;

    public GeneticAlgorithm(
        int a,
        int b,
        int populationPercentage,
        int mutationPercentage,
        double chanceOfMutation)
    {
        A = a;
        B = b;
        PopulationPercentage = populationPercentage;
        MutationPercentage = mutationPercentage;
        ChanceOfMutation = chanceOfMutation;

        (BitQuantities, QuantityOfChromosomes) = GetQuantity();
    }

    public void GetMinimum()
    {
        LoadFenotypes();

        GetNewPopulation(true);

        _bestFenotype = Fenotypes
            .OrderBy(f => f.FunctionValue)
            .First();

        const int maxGenerations = 100;

        for (int generation = 0; generation < maxGenerations; generation++)
        {

            GetNewPopulation(false);

            var currentBest = Fenotypes
                .OrderBy(f => f.FunctionValue)
                .First();

            if (currentBest.FunctionValue < _bestFenotype.FunctionValue)
            {
                _bestFenotype = currentBest;
            }
        }

        Console.WriteLine(
            $"Answer is: {_bestFenotype.Value}, " +
            $"Function value: {_bestFenotype.FunctionValue}");
    }

    public void GetNewPopulation(bool isFirstRandom = false)
    {
        if (isFirstRandom)
        {
            var selectedIndexes = new HashSet<int>();
            var result = new List<Fenotype>();

            while (result.Count < QuantityOfChromosomes)
            {
                int index = _random.Next(0, InitialPopulation.Count);

                if (selectedIndexes.Add(index))
                {
                    result.Add(InitialPopulation[index]);
                }
            }

            Fenotypes = result;
        }

        // Selection
        ParentsCalculation(out var chunkedResult);

        // Crossover
        Krosover(ref chunkedResult);

        // Mutation
        Mutation(ref chunkedResult);

        var newPopulation = chunkedResult
            .SelectMany(x => x)
            .ToList();

        // Updating Data
        UpdateFenotypes(ref newPopulation);

        var previousBest = Fenotypes
            .OrderBy(f => f.FunctionValue)
            .First();

        var worstIndex = newPopulation
            .Select((f, index) => new { Fenotype = f, Index = index })
            .OrderByDescending(x => x.Fenotype.FunctionValue)
            .First()
            .Index;

        if (previousBest.FunctionValue <
            newPopulation[worstIndex].FunctionValue)
        {
            newPopulation[worstIndex] = new Fenotype
            {
                Value = previousBest.Value,
                Chromosome = new BitArray(previousBest.Chromosome),
                FunctionValue = previousBest.FunctionValue,
                Coefficient = previousBest.Coefficient
            };
        }

        Fenotypes = newPopulation;
    }

    public int BitArrayToInt(BitArray bitArray)
    {
        int offset = 0;

        for (int i = 0; i < bitArray.Count; i++)
        {
            if (bitArray[i])
            {
                offset |= (1 << i);
            }
        }

        int value = A + offset;

        if (value < A) value = A;
        if (value > B) value = B;

        return value;
    }

    public void UpdateFenotypes(ref List<Fenotype> result)
    {
        for (int i = 0; i < result.Count; i++)
        {
            int value = BitArrayToInt(result[i].Chromosome);
            double fitness = GetFitness(value);
            double coefficient = 1 / (fitness + 0.000001);

            result[i] = new Fenotype
            {
                Value = value,
                FunctionValue = fitness,
                Coefficient = coefficient,
                Chromosome = result[i].Chromosome
            };
        }
    }

    public void ParentsCalculation(out List<Fenotype[]> chunkedResult)
    {
        var sumOfCoefficient = Fenotypes.Sum(f => f.Coefficient);

        for (int i = 0; i < Fenotypes.Count; i++)
        {
            double expectedQuantity =
                Fenotypes[i].Coefficient /
                sumOfCoefficient *
                Fenotypes.Count;

            Fenotypes[i] = new Fenotype
            {
                Value = Fenotypes[i].Value,
                Chromosome = Fenotypes[i].Chromosome,
                FunctionValue = Fenotypes[i].FunctionValue,
                Coefficient = Fenotypes[i].Coefficient,

                ExpectedQuantityAtFathersArray =
                    Fenotypes[i].Coefficient / sumOfCoefficient,
                
                ChosenQuantityAtFathersArray =
                    (int)Math.Floor(expectedQuantity)
            };
        }

        int chosenCount = Fenotypes.Sum(
            f => f.ChosenQuantityAtFathersArray);

        int remainingCount =
            Fenotypes.Count - chosenCount;


        //If there is any remainders it will calculate new gens
        var fractions = Fenotypes
            .Select((f, index) =>
            {
                double expectedQuantity =
                    f.ExpectedQuantityAtFathersArray *
                    Fenotypes.Count;

                double fraction =
                    expectedQuantity -
                    Math.Floor(expectedQuantity);

                return new
                {
                    Index = index,
                    Fraction = fraction
                };
            })
            .OrderByDescending(x => x.Fraction)
            .ToList();

        for (int i = 0; i < remainingCount; i++)
        {
            int index = fractions[i].Index;
            var fenotype = Fenotypes[index];
            fenotype.ChosenQuantityAtFathersArray++;
            Fenotypes[index] = fenotype;
        }
        

        var newChromosomes = new List<Fenotype>();

        for (int i = 0; i < Fenotypes.Count; i++)
        {
            for (
                int j = 0;
                j < Fenotypes[i].ChosenQuantityAtFathersArray;
                j++)
            {
                newChromosomes.Add(new Fenotype
                {
                    Value = Fenotypes[i].Value,

                    Chromosome =
                        new BitArray(Fenotypes[i].Chromosome),

                    FunctionValue =
                        Fenotypes[i].FunctionValue,

                    Coefficient =
                        Fenotypes[i].Coefficient,

                    ExpectedQuantityAtFathersArray =
                        Fenotypes[i].ExpectedQuantityAtFathersArray,

                    ChosenQuantityAtFathersArray =
                        Fenotypes[i].ChosenQuantityAtFathersArray
                });
            }
        }


        chunkedResult = newChromosomes
            .Chunk(2)
            .ToList();
    }

    public void Mutation(ref List<Fenotype[]> chunkedResult)
    {
        if (chunkedResult.Count == 0)
            return;

        if (_random.NextDouble() < ChanceOfMutation)
        {
            var quantityOfMutations = (int)Math.Round(
                (
                    chunkedResult.Count *
                    chunkedResult.First().Length *
                    chunkedResult.First().First().Chromosome.Count
                ) / (double)MutationPercentage,
                MidpointRounding.ToEven);

            for (int i = 0; i < quantityOfMutations; i++)
            {
                var randomGroup =
                    _random.Next(0, chunkedResult.Count);

                var randomChromosome =
                    _random.Next(
                        0,
                        chunkedResult[randomGroup].Length);

                var randomGen =
                    _random.Next(
                        0,
                        chunkedResult[randomGroup]
                            [randomChromosome]
                            .Chromosome
                            .Length);

                chunkedResult[randomGroup]
                    [randomChromosome]
                    .Chromosome[randomGen] =
                        !chunkedResult[randomGroup]
                            [randomChromosome]
                            .Chromosome[randomGen];
            }
        }
    }

    public void Krosover(ref List<Fenotype[]> chunkedResult)
    {
        foreach (var f in chunkedResult)
        {
            if (f.Length == 1)
                continue;

            var point = _random.Next(1, BitQuantities);

            for (int i = point; i < BitQuantities; i++)
            {
                (
                    f[0].Chromosome[i],
                    f[1].Chromosome[i]
                ) =
                (
                    f[1].Chromosome[i],
                    f[0].Chromosome[i]
                );
            }
        }
    }

    public void LoadFenotypes()
    {
        for (int i = A; i <= B; i++)
        {
            int offset = i - A;

            var chromosome =
                new BitArray(new int[] { offset });

            chromosome.Length = BitQuantities;

            var fitness = GetFitness(i);

            var coefficient =
                1 / (fitness + 0.000001);

            InitialPopulation.Add(new Fenotype
            {
                Value = i,
                Chromosome = chromosome,
                FunctionValue = fitness,
                Coefficient = coefficient,
            });
        }
    }

    public double GetFitness(int x) => x * x;

    public (int, int) GetQuantity()
    {
        var rangeSize =
            Math.Abs(B - A) + 1;

        for (int i = 1; ; i++)
        {
            if (Math.Pow(2.0, i) >= rangeSize)
            {
                return (
                    i,
                    (int)Math.Round(
                        rangeSize *
                        (PopulationPercentage / 100.0),
                        MidpointRounding.ToEven)
                );
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}