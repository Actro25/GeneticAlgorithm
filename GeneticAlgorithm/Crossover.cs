namespace GeneticAlgorithm;

public interface ICrossover
{
    int BitQuantities { get; set; }
    public void CrossoverWithOnePoint(List<Phenotype> phenotypes, ref List<Phenotype[]> chunkedResult);
}

public class Crossover : ICrossover
{
    public int BitQuantities { get; set; }
    private static readonly Random _random = new Random();
    
    public void CrossoverWithOnePoint(List<Phenotype> phenotypes, ref List<Phenotype[]> chunkedResult)
    {
        foreach (var f in chunkedResult)
        {
            //If there is last element that doesn't have pair we just skep him.
            if (f.Length == 1)
                continue;

            //The point where we are start from.
            //Here we're getting random point from 1 to bitQuantities.
            var point = _random.Next(1, BitQuantities);

            //Here we're doing Krosover to the point.
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
}