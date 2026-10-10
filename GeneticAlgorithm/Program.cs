using GeneticAlgorithm.GeneticAlgorithmImplementation.Classes;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Classes.Debug;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm;

public class MyGeneticAlgorithm1 : GeneticAlgorithmImplementation.GeneticAlgorithm
{
    public MyGeneticAlgorithm1(
        List<(double a, double b)> distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null) : base(distance, precision, phenotypesCalculation, crossover, mutation, debug)
    {
    }

    public override double GetFitness(List<double> point)
    {
        return point.First() * point.First();
    }

    public override double GetCoefficient(double fitness)
    {
        return 1.0 / (1.0 + fitness);
    }
}

public class MyGeneticAlgorithm2 : GeneticAlgorithmImplementation.GeneticAlgorithm
{
    public MyGeneticAlgorithm2(
        List<(double a, double b)> distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null) : base(distance, precision, phenotypesCalculation, crossover, mutation, debug)
    {
    }

    public override double GetFitness(List<double> point)
    {
        return 0.2 * Math.Pow(point.First(), 4) + 0.3 * Math.Pow(point.First(), 3) + (-2.4 * Math.Pow(point.First(), 2)) + (-0.6 * point.First()) + 20;
    }

    public override double GetCoefficient(double fitness)
    {
        return 1.0 / (1.0 + fitness);
    }
}

public class MyGeneticAlgorithm3 : GeneticAlgorithmImplementation.GeneticAlgorithm
{
    public MyGeneticAlgorithm3(
        List<(double a, double b)> distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null) : base(distance, precision, phenotypesCalculation, crossover, mutation, debug)
    {
    }

    public override double GetFitness(List<double> point)
    {
        return Math.Pow(Math.Pow(point[0], 2) + Math.Pow(point[1], 2), 1.0/3.0);
    }

    public override double GetCoefficient(double fitness)
    {
        return 1.0 / (1.0 + fitness);
    }
}

public class MyGeneticAlgorithm4 : GeneticAlgorithmImplementation.GeneticAlgorithm
{
    public MyGeneticAlgorithm4(
        List<(double a, double b)> distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null) : base(distance, precision, phenotypesCalculation, crossover, mutation, debug)
    {
    }

    public override double GetFitness(List<double> point)
    {
        double dx = point[0] - -1;
        double dy = point[1] - 1;

        return 1 * Math.Pow(dx, 4) 
               + 1 * Math.Pow(dy, 4) 
               + -2 * Math.Pow(dx, 3) 
               + 2 * Math.Pow(dy, 2) 
               + -10 * dx * dy 
               + 0.25 * dx 
               + 20;
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
        var distances = new List<(double A, double B)>();
        double precision, chanceOfMutation, populationPercentage, quantityOfMutation;
        int loops;
        bool isTableMod;
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        var style = System.Globalization.NumberStyles.Float;

        while (true)
        {
            if (distances.Count > 0)
            {
                Console.WriteLine("\n┌────────────────────────────────────────┐");
                Console.WriteLine("│       Current Entered Dimensions       │");
                Console.WriteLine("├────────┬────────────────┬──────────────┤");
                Console.WriteLine("│ Axis № │      Min (A)   │    Max (B)   │");
                Console.WriteLine("├────────┼────────────────┼──────────────┤");
                for (int i = 0; i < distances.Count; i++)
                {
                    Console.WriteLine("│ {0,-6} │ {1,14:F4} │ {2,12:F4} │", i + 1, distances[i].A, distances[i].B);
                }
                Console.WriteLine("└────────┴────────────────┴──────────────┘\n");
            }

            Console.WriteLine($"--- Configuring Axis {distances.Count + 1} ---");

            double a, b;

            while (true)
            {
                Console.Write($"Enter A for axis {distances.Count + 1}: ");
                string text = (Console.ReadLine() ?? "").Replace(',', '.');
                if (double.TryParse(text, style, inv, out a) && double.IsFinite(a)) 
                    break;
                Console.WriteLine("Error: enter a valid number, for example -5 or 0.5.");
            }

            while (true)
            {
                Console.Write($"Enter B for axis {distances.Count + 1} (must be greater than {a}): ");
                string text = (Console.ReadLine() ?? "").Replace(',', '.');
                if (double.TryParse(text, style, inv, out b) && double.IsFinite(b) && b > a) 
                    break;
                Console.WriteLine("Error: enter a number greater than A.");
            }

            distances.Add((a, b));

            Console.Write("\nDo you want to add another axis? (y/n): ");
            string? answer = Console.ReadLine()?.Trim().ToLower();
        
            if (answer != "y" && answer != "yes")
            {
                break;
            }
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

        while (true)
        {
            Console.Write("Enter quantity of loops that will be done to conclude result: ");
            string text = (Console.ReadLine() ?? "");
            if (!int.TryParse(text, style, inv, out loops) || loops <= 0)
            {
                Console.WriteLine("Error: enter a number greater than 0");
                continue;
            }
            break;
        }
        
        while (true)
        {
            Console.Write("Do you want see tables? (false / true, If you enter true program will be much slower and will be waiting for your actions): ");
            string text = (Console.ReadLine() ?? "");
            if (!bool.TryParse(text, out isTableMod))
            {
                Console.WriteLine("Error: enter only false / true");
                continue;
            }
            break;
        }

        var quantityOfChromosome = GetQuantityOfChromosome(populationPercentage, distances, precision);

        IPhenotypesCalculation phenotypesCalculation = new PhenotypesCalculation(quantityOfChromosome);
        ICrossover crossover = new Crossover(isTableMod ? new CrossoverDebug(2) : null);
        IMutation mutation = new Mutation(quantityOfMutation, chanceOfMutation, isTableMod ? new MutationDebug() : null);
        IGeneticAlgorithmDebug? debug =  isTableMod ? new GeneticAlgorithmDebug() : null;
        
        GeneticAlgorithmImplementation.GeneticAlgorithm ga = new MyGeneticAlgorithm4(distances, precision, phenotypesCalculation, crossover, mutation, debug);
        
        ga.GetMinimum(loops);
    }

    static int GetQuantityOfChromosome(double populationPercentage, List<(double A, double B)> distances, double precision)
    {
        var totalStateSpace = 1D;
        foreach (var distance in distances)
        {
            double pointsPerAxis = (Math.Abs(distance.B - distance.A) / precision) + 1;
            totalStateSpace *= pointsPerAxis;
        }

        return (int)Math.Round(
            totalStateSpace * populationPercentage,
            MidpointRounding.ToEven);
    }
}