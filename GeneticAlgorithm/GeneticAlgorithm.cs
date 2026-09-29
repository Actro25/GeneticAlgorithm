using System.Collections;
namespace GeneticAlgorithm;

public struct Phenotype
{
    public int Value;
    public BitArray Chromosome;
    public double FunctionValue;
    public double Coefficient;
}

public interface ICustomizableGeneticAlgorithm
{
    public double GetFitness(int x);

    public double GetCoefficient(double fitness);
}

public interface IGeneticAlgorithm
{
    (int A, int B) Distance { get; set; }
    List<Phenotype> InitialPopulation { get; set; }
}

public abstract class GeneticAlgorithmNew : IGeneticAlgorithm {
    public (int A, int B) Distance { get; set; }
    public List<Phenotype> Phenotypes { get; set; } = [];
    public List<Phenotype> InitialPopulation { get; set; } = [];
    
    private ICustomizableGeneticAlgorithm _functions;
    private IPhenotypesCalculation _phenotypesCalculation;
    private ICrossover _crossover;
    private IMutation _mutation;
    
    private static readonly Random _random = new Random();
    private Phenotype _bestPhenotype;
    
    protected GeneticAlgorithmNew(
        (int a, int b) distance,
        ICustomizableGeneticAlgorithm functions,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation
        )
    {
        Distance = distance;
        
        _functions = functions;
        _phenotypesCalculation = phenotypesCalculation;
        _crossover = crossover;
        _mutation = mutation;
    }
    
    public void GetMinimum(int maxGenerations = 1000)
    {
        LoadPhenotypes();

        GetNewPopulation(true);

        _bestPhenotype = Phenotypes
            .OrderBy(f => f.FunctionValue)
            .First();

        for (int generation = 0; generation < maxGenerations; generation++)
        {
            GetNewPopulation(false);

            var currentBest = Phenotypes
                .OrderBy(f => f.FunctionValue)
                .First();

            if (currentBest.FunctionValue < _bestPhenotype.FunctionValue)
            {
                _bestPhenotype = currentBest;
            }
        }

        Console.WriteLine(
            $"Answer is: {_bestPhenotype.Value}, " +
            $"Function value: {_bestPhenotype.FunctionValue}");
    }
    
    public void GetNewPopulation(bool isFirstRandom = false, bool areWeLookingForMinimum = true)
    {
        if (isFirstRandom)
        {
            var selectedIndexes = new HashSet<int>();
            var result = new List<Phenotype>();

            while (result.Count < _phenotypesCalculation.QuantityOfChromosomes)
            {
                int index = _random.Next(0, InitialPopulation.Count);

                if (selectedIndexes.Add(index))
                {
                    result.Add(InitialPopulation[index]);
                }
            }

            Phenotypes = result;
        }

        // Selection
        _phenotypesCalculation.CalculatePhenotypes(Phenotypes, out var chunkedResult);

        // Crossover
        _crossover.CrossoverWithOnePoint(Phenotypes, ref chunkedResult);;

        // Mutation
        _mutation.MutateOneGen(ref chunkedResult);

        var newPopulation = chunkedResult
            .SelectMany(x => x)
            .ToList();

        // Updating Data
        _phenotypesCalculation.UpdatePhenotypes(newPopulation, _functions, Distance);
        
        var previousBest = areWeLookingForMinimum
            ? Phenotypes.OrderBy(f => f.FunctionValue).First()
            : Phenotypes.OrderByDescending(f => f.FunctionValue).First();

        var worstIndex = areWeLookingForMinimum
            ? newPopulation
                .Select((f, index) => new { Phenotype = f, Index = index })
                .OrderByDescending(x => x.Phenotype.FunctionValue)
                .First()
                .Index
            : newPopulation
                .Select((f, index) => new { Phenotype = f, Index = index })
                .OrderBy(x => x.Phenotype.FunctionValue)
                .First()
                .Index;

        if (areWeLookingForMinimum
                ? previousBest.FunctionValue < newPopulation[worstIndex].FunctionValue
                : previousBest.FunctionValue > newPopulation[worstIndex].FunctionValue)
        {
            newPopulation[worstIndex] = new Phenotype
            {
                Value = previousBest.Value,
                Chromosome = new BitArray(previousBest.Chromosome),
                FunctionValue = previousBest.FunctionValue,
                Coefficient = previousBest.Coefficient
            };
        }

        Phenotypes = newPopulation;
    }
    
    private void LoadPhenotypes()
    {
        for (int i = Distance.A; i <= Distance.B; i++)
        {
            int offset = i - Distance.A;

            var chromosome =
                new BitArray(new int[] { offset });

            chromosome.Length = _crossover.BitQuantities;

            var fitness = _functions.GetFitness(i);

            var coefficient = _functions.GetCoefficient(fitness);

            InitialPopulation.Add(new Phenotype
            {
                Value = i,
                Chromosome = chromosome,
                FunctionValue = fitness,
                Coefficient = coefficient,
            });
        }
    }
}