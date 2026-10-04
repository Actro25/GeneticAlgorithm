using GeneticAlgorithm.GeneticAlgorithmImplementation.Classes;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Classes.Debug;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm;

public class MyGeneticAlgorithm : GeneticAlgorithmImplementation.GeneticAlgorithm
{
    public MyGeneticAlgorithm(
        (double a, double b) distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null) : base(distance, precision, phenotypesCalculation, crossover, mutation, debug)
    {
    }

    public override double GetFitness(double x)
    {
        return x * x;
    }

    public override double GetCoefficient(double fitness)
    {
        return 1.0 / (1.0 + fitness);
    }
}

class Program
{
    static void Main(string[] args)
    {
        InputData();
    }

static void InputData()
{
    double a, b, precision, chanceOfMutation, populationPercentage, quantityOfMutation;
    var inv = System.Globalization.CultureInfo.InvariantCulture;
    var style = System.Globalization.NumberStyles.Float;

    while (true)
    {
        Console.Write("Enter A: ");
        string text = (Console.ReadLine() ?? "").Replace(',', '.');
        if (double.TryParse(text, style, inv, out a) && double.IsFinite(a)) break;
        Console.WriteLine("Error: enter a number, for example -5 or 0.5.");
    }

    while (true)
    {
        Console.Write("Enter B (greater than A): ");
        string text = (Console.ReadLine() ?? "").Replace(',', '.');
        if (double.TryParse(text, style, inv, out b) && double.IsFinite(b) && b > a) break;
        Console.WriteLine("Error: enter a number greater than A.");
    }

    while (true)
    {
        Console.Write("Enter size of population that will be at the next populations (0.1 = 10% etc., number has to be greater than 0): ");
        string text = (Console.ReadLine() ?? "").Replace(',', '.');
        if (double.TryParse(text, out populationPercentage)
            && populationPercentage is > 0 and <= 1) break;
        Console.WriteLine("Error: enter a whole number from 0 to 1.");
    }

    while (true)
    {
        Console.Write("Enter a size of population that will be mutated (0.1 = 10% etc.): ");
        string text = (Console.ReadLine() ?? "").Replace(',', '.');
        if (double.TryParse(text, out quantityOfMutation)
            && quantityOfMutation is >= 0 and <= 1) break;
        Console.WriteLine("Error: enter a whole number from 0 to 1.");
    }

    while (true)
    {
        Console.Write("Enter a chance of mutation (0.1 = 10% etc.): ");
        string text = (Console.ReadLine() ?? "").Replace(',', '.');
        if (double.TryParse(text, style, inv, out chanceOfMutation)
            && chanceOfMutation is >= 0 and <= 1) break;
        Console.WriteLine("Error: enter a number from 0 to 1, for example 0.1.");
    }

    while (true)
    {
        Console.Write("Enter precision - step between values (if you don't know what it is, just enter 1): ");
        string text = (Console.ReadLine() ?? "").Replace(',', '.');
        if (!double.TryParse(text, style, inv, out precision) || !double.IsFinite(precision) || precision <= 0)
        {
            Console.WriteLine("Error: enter a number greater than 0, for example 1 or 0.01.");
            continue;
        }
        break;
    }

    var distance = (A: a, B: b);
    var (bitQuantity, quantityOfChromosome) = GetQuantity(populationPercentage, distance, precision);

    IPhenotypesCalculation phenotypesCalculation = new PhenotypesCalculation(quantityOfChromosome);
    ICrossover crossover = new Crossover(bitQuantity, new CrossoverDebug(2));
    IMutation mutation = new Mutation(quantityOfMutation, chanceOfMutation, new MutationDebug());
    IGeneticAlgorithmDebug debug = new GeneticAlgorithmDebug();
    
    GeneticAlgorithmImplementation.GeneticAlgorithm ga = new MyGeneticAlgorithm(distance, precision, phenotypesCalculation, crossover, mutation, debug);
    ga.GetMinimum();
}

    static (int, int) GetQuantity(double populationPercentage, (double A, double B) distance, double precision)
    {
        var rangeSize = (Math.Abs(distance.B - distance.A) / precision) + 1;

        for (int i = 1; ; i++)
        {
            if (Math.Pow(2.0, i) >= rangeSize)
            {
                return (
                    i,
                    (int)Math.Round(
                        rangeSize * populationPercentage,
                        MidpointRounding.ToEven)
                );
            }
        }
    }
}