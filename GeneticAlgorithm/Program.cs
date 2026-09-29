namespace GeneticAlgorithm;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
    
    private (int, int) GetQuantity1(int populationPercentage, (int A, int B) distance)
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