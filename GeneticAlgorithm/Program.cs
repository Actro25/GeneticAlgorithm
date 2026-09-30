namespace GeneticAlgorithm;

public class MyGeneticAlgorithm : GeneticAlgorithmNew
{
    public MyGeneticAlgorithm(
        (int a, int b) distance,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation) : base(distance, phenotypesCalculation, crossover, mutation)
    {
    }

    public override double GetFitness(int x)
    {
        return Math.Pow(x * x - 25, 2);
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
        int a, b, populationPercentage, quantityOfMutation;
        double chanceOfMutation;

        while (true)
        {
            Console.Write("Enter A: ");
            if (int.TryParse(Console.ReadLine(), out a)) break;
            Console.WriteLine("Error: enter a whole number.");
        }

        while (true)
        {
            Console.Write("Enter B (greater than A): ");
            if (int.TryParse(Console.ReadLine(), out b) && b > a) break;
            Console.WriteLine("Error: enter a whole number greater than A.");
        }

        while (true)
        {
            Console.Write("Enter size of population (1-100 % of range): ");
            if (int.TryParse(Console.ReadLine(), out populationPercentage)
                && populationPercentage is >= 1 and <= 100) break;
            Console.WriteLine("Error: enter a whole number from 1 to 100.");
        }

        while (true)
        {
            Console.Write("Enter a percentage of population that will be mutated (0-100): ");
            if (int.TryParse(Console.ReadLine(), out quantityOfMutation)
                && quantityOfMutation is >= 0 and <= 100) break;
            Console.WriteLine("Error: enter a whole number from 0 to 100.");
        }

        while (true)
        {
            Console.Write("Enter a chance of mutation (0.1 = 10%, from 0 to 1): ");
            string text = (Console.ReadLine() ?? "").Replace(',', '.');
            if (double.TryParse(text, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out chanceOfMutation)
                && chanceOfMutation is >= 0 and <= 1) break;
            Console.WriteLine("Error: enter a number from 0 to 1, for example 0.1.");
        }

        (int a, int b) distance = (a, b);
        var (bitQuantity, quantityOfChromosome) = GetQuantity(populationPercentage, distance);
        quantityOfChromosome = Math.Max(10, quantityOfChromosome);

        IPhenotypesCalculation phenotypesCalculation = new PhenotypesCalculation(quantityOfChromosome);
        ICrossover crossover = new Crossover(bitQuantity);
        IMutation mutation = new Mutation(quantityOfMutation, chanceOfMutation);

        GeneticAlgorithmNew ga = new MyGeneticAlgorithm(distance, phenotypesCalculation, crossover, mutation);
        ga.GetMinimum();
    }

    static (int, int) GetQuantity(int populationPercentage, (int A, int B) distance)
    {
        var rangeSize =
            Math.Abs(distance.B - distance.A) + 1;

        for (int i = 1; ; i++)
        {
            if (Math.Pow(2.0, i) >= rangeSize)
            {
                return (
                    i,
                    (int)Math.Round(
                        rangeSize *
                        (populationPercentage / 100.0),
                        MidpointRounding.ToEven)
                );
            }
        }
    }
}