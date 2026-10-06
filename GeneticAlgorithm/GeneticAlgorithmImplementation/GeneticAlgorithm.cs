using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation;

public struct Phenotype
{
    public double Value;
    public BitArray Chromosome;
    public double FunctionValue;
    public double Coefficient;
}

public interface IFunctionGeneticAlgorithm
{
    public double Precision { get; set; }
    public double GetFitness(double x);
    public void DebugPhenotypes(List<Phenotype> phenotypes, string title);
    public double GetCoefficient(double fitness);
}

public interface IGeneticAlgorithm
{
    (double A, double B) Distance { get; set; }
    List<Phenotype> InitialPopulation { get; set; }
}

public abstract class GeneticAlgorithm : IGeneticAlgorithm, IFunctionGeneticAlgorithm {
    public (double A, double B) Distance { get; set; }
    public double Precision { get; set; } = 1;
    public List<Phenotype> Phenotypes { get; set; } = [];
    public List<Phenotype> InitialPopulation { get; set; } = [];
    
    private readonly IPhenotypesCalculation _phenotypesCalculation;
    private readonly ICrossover _crossover;
    private readonly IMutation _mutation;
    private readonly IGeneticAlgorithmDebug? _debug;
    
    private readonly Random _random = new Random();
    
    private Phenotype _bestPhenotype;
    
    protected GeneticAlgorithm(
        (double a, double b) distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null
        )
    {
        Distance = distance;
        Precision = precision;
        
        _phenotypesCalculation = phenotypesCalculation;
        _crossover = crossover;
        _mutation = mutation;
        _debug = debug;
    }
    
    public void GetMinimum(int maxGenerations = 1000)
    {
        LoadPhenotypes();
        
        DebugPhenotypes(InitialPopulation, "Initial population");
        
        GetNewPopulation(true);
        
        DebugPhenotypes(Phenotypes, "№0 Generation");
        
        _bestPhenotype = Phenotypes
            .OrderBy(f => f.FunctionValue)
            .First();

        for (int generation = 1; generation < maxGenerations; generation++)
        {
            GetNewPopulation(false);
            
            DebugPhenotypes(Phenotypes, $"№{generation} Generation");
            
            var currentBest = Phenotypes.OrderBy(f => f.FunctionValue).First();

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
                result.Add(InitialPopulation[_random.Next(InitialPopulation.Count)]);
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
        _phenotypesCalculation.UpdatePhenotypes(newPopulation, this, Distance, Precision);
        
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

    public void DebugPhenotypes(List<Phenotype> phenotypes, string title)
    {
        if(_debug != null)
        {
            _debug.PrintPhenotypes(phenotypes, title);
            Console.Write("To continue to the next iteration click at any button...");
            Console.ReadLine();
            Console.WriteLine("");
        }
    }

    private void LoadPhenotypes()
    {
        InitialPopulation.Clear();

        double range = Math.Abs(Distance.B - Distance.A);
        int totalSteps = (int)Math.Ceiling(range / Precision);

        for (int offset = 0; offset <= totalSteps; offset++)
        {
            //Calculation real X value considering Precision.
            double x = Distance.A + (offset * Precision);
        
            if (x > Distance.B) x = Distance.B;

            /*
             * There we convert offset into BitArray for algorithm.
             * Why is offset? Because there is something wierd with negative numbers, so
             * I decided to use offset as id of X. If the X is his value then offset is his id.
             * The offset is always positive and there isn't any problem with it.
             */
            var chromosome = new BitArray(new int[] { offset })
            {
                Length = _crossover.BitQuantities
            };

            double fitness = GetFitness(x);
            double coefficient = GetCoefficient(fitness);

            InitialPopulation.Add(new Phenotype
            {
                Value = x, 
                Chromosome = chromosome,
                FunctionValue = fitness,
                Coefficient = coefficient,
            });
        }
    }
    
    public abstract double GetFitness(double x);
    public abstract double GetCoefficient(double fitness);
}