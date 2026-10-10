using System.Collections;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;
using GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation;

public struct Phenotype
{
    public List<double> Vector;
    public BitArray Chromosome;
    public double FunctionValue;
    public double Coefficient;
    
    public Phenotype Clone()
    {
        return new Phenotype
        {
            Chromosome = new BitArray(this.Chromosome),
            Vector = new List<double>(this.Vector),
            FunctionValue = this.FunctionValue,
            Coefficient = this.Coefficient
        };
    }
}

public interface IFunctionGeneticAlgorithm
{
    public double GetFitness(List<double> point);
    public void DebugPhenotypes(List<Phenotype> phenotypes, string title);
    public double GetCoefficient(double fitness);
}


public abstract class GeneticAlgorithm : IFunctionGeneticAlgorithm {

    
    private List<Phenotype> _phenotypes = [];
    private readonly List<(double A, double B)> _distance;
    private List<List<double>> _sequence = [];
    private List<int> _bitLengths = [];
    private double Precision { get; set; }

    
    private readonly IPhenotypesCalculation _phenotypesCalculation;
    private readonly ICrossover _crossover;
    private readonly IMutation _mutation;
    private readonly IGeneticAlgorithmDebug? _debug;
    
    private readonly Random _random = new Random();
    
    private Phenotype _bestPhenotype;
    
    protected GeneticAlgorithm(
        List<(double a, double b)> distance,
        double precision,
        IPhenotypesCalculation phenotypesCalculation,
        ICrossover crossover,
        IMutation mutation,
        IGeneticAlgorithmDebug? debug = null
        )
    {
        _distance = distance;
        Precision = precision;
        
        _phenotypesCalculation = phenotypesCalculation;
        _crossover = crossover;
        _mutation = mutation;
        _debug = debug;
    }
    
    public void GetMinimum(int maxGenerations = 1000)
    {
        LoadPhenotypes();
        
        DebugPhenotypes(_phenotypes, "Initial population");
        
        GetNewPopulation(true, true);
        
        DebugPhenotypes(_phenotypes, "№0 Generation");
        
        _bestPhenotype = _phenotypes
            .OrderBy(f => f.FunctionValue)
            .First();

        for (int generation = 1; generation < maxGenerations; generation++)
        {
            GetNewPopulation(false, true);
            
            DebugPhenotypes(_phenotypes, $"№{generation} Generation");
            
            var currentBest = _phenotypes.OrderBy(f => f.FunctionValue).First();

            if (currentBest.FunctionValue < _bestPhenotype.FunctionValue)
            {
                _bestPhenotype = currentBest;
            }
        }

        var vectorAnswer = "";
        for(int i = 0; i < _bestPhenotype.Vector.Count; i++)
        {
            if (i == _bestPhenotype.Vector.Count - 1)
            {
                vectorAnswer += $"{_bestPhenotype.Vector[i]}";
            }
            else
            {
                vectorAnswer += $"{_bestPhenotype.Vector[i]}, ";
            }
        }

        Console.WriteLine(
            $"Answer is: {vectorAnswer}, " +
            $"Function value: {_bestPhenotype.FunctionValue}");
    }
    
    private void GetNewPopulation(bool isFirstRandom, bool areWeLookingForMinimum)
    {
        if (isFirstRandom)
        {
            // Here can be chosen identical phenotypes!
            var result = new List<Phenotype>();
            while (result.Count < _phenotypesCalculation.QuantityOfChromosomes)
            {
                result.Add(_phenotypes[_random.Next(_phenotypes.Count)]);
            }

            _phenotypes = result;
        }

        // Selection
        _phenotypesCalculation.CalculatePhenotypes(_phenotypes, out var chunkedResult);

        // Crossover
        _crossover.CrossoverBits(_phenotypes, ref chunkedResult);;

        // Mutation
        _mutation.Mutate(ref chunkedResult);

        var newPopulation = chunkedResult
            .SelectMany(x => x)
            .ToList();

        // Updating Data
        _phenotypesCalculation.UpdatePhenotypes(newPopulation, this, _distance, _sequence, _bitLengths);
        
        var previousBest = areWeLookingForMinimum
            ? _phenotypes.OrderBy(f => f.FunctionValue).First()
            : _phenotypes.OrderByDescending(f => f.FunctionValue).First();

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
            newPopulation[worstIndex] = previousBest with 
            { 
                Chromosome = new BitArray(previousBest.Chromosome),
                Vector = [.. previousBest.Vector]
            };
        }

        _phenotypes = newPopulation;
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

    /// <summary>
    /// This function just loads and calculates Phenotype.
    /// </summary>
    private void LoadPhenotypes()
    {
        /*
         * The variable sequence contains points inside distance with precision.
         * The selec iterates in dimensions because each distance created for their coordinate (dimension).
         * For example:
         *          distance = {-2; 2} - this is a distance between two points at some coordinate (dimension).
         *          sequence[k] = {-2, -1, 0, 1, 2} - this is ready points for certain dimension with precision = 1.
         */
        var sequence = _distance.Select(d =>
        {
            var result = new List<double>();
            int count = (int)Math.Round((d.B - d.A) / Precision) + 1;
            for (int i = 0; i < count; i++)
            {
                double val = d.A + i * Precision;
                if (val > d.B) val = d.B;
                result.Add(val);
            }

            return result;
        }).ToList();
        
        /*
         * Here we get bits length for each dimension.
         * It needs for creating chromosome. Because to code chromosome we need known quantity of bits for each coordinate (dimension).
         * For example:
         *          sequence[k] = {-2, -1, 0, 1, 2} - this is ready points for certain dimension with precision = 1.
         *          bitLengths[k] = 3 - it means we need 3 bits to code the certain coordinate.
         *
         * Also, here we code index not values. sequence[k] have 5 elements so to code index 5 from k dimension we need at least 3 bits.
         * The bits we get from bitLengths[k] where k - it's a dimension. 
         */
        List<int> bitLengths = sequence
            .Select(list => (int)Math.Ceiling(Math.Log2(list.Count)))
            .ToList();
        
        /*
         * Here we get bits for our points.
         * For example:
         *          x = 3 bits
         *          y = 3 bits
         *          z = 2 bits
         *
         * These bits represent their the biggest index to code it.
         * But to code chromosome we need to know total bits for all points.
         * For example:
         *          chromosome = |0   0   1|   |0   1   1|   |0   0| -> 8 bits - to code chromosome
         *
         * So the first 3 bits represent X, next three bits represent y and so on till K dimensions.
         */
        int totalBitLength = bitLengths.Sum();
        
        _crossover.BitQuantities = totalBitLength;
        
        /*
         * Here we get our actual points. NOT PHENOTYPES!
         * To get actual points from some quantity of dimensions we have to find a discrete multiplication by sequence.
         * Sequence contains our points that isn't combined.
         * For example:
         *           sequence[0] = {-1, -0.5, 0, 0.5, 1} - this will represent x coordinate.
         *           sequence[1] = {-1.5, -1, -0.5, 0, 0.5, 1, 1.5} - this will represent y coordinate.
         *           ...
         *           sequence[k] = {-0.5, 0, 0.5, 1, 1.5, 2} - this will represent k dimension (coordinate).
         *
         * So we have k dimensions (coordinates) with points at their coordinates, we need to combine all them to find real points considering all dimensions.
         */ 
        var readyPoints = GetNewVector(sequence);

        /*
         * Here we iterate in readyPoints to create Phenotypes.
         * Also, here will be created chromosome, fitness value, coefficient value.
         */
        foreach (var pointVector in readyPoints)
        {
            /*
             * This offset needs for creating chromosome.
             * Look at chromosome comment to find more.
             */
            var offset = 0;
            var chromosome = new BitArray(totalBitLength);

            /*
             * Here we iterate through dimensions to create chromosome.
             * Currently, pointVector is a vector of points that describe one point in space.
             * For example:
             *          pointVector = {
             *              2.3, -> represent coordinate x
             *              -4.0, -> represent coordinate y
             *              ...
             *              0.2 -> represent k's dimension
             *          }
             */
            for (int dimension = 0; dimension < pointVector.Count; dimension++)
            {
                double value = pointVector[dimension];
                int bitLength = bitLengths[dimension];

                /*
                 * Here we've got index from dimension and value.
                 * Value it is representation of a coordinate. (Look above for example)
                 * So here we look at dimension in sequence and find index of the value in list.
                 * For example:
                 *          sequence[0] = {-1, -0.5, 0, 0.5, 1},
                 *          value = 0.5,
                 *          dimension = 0
                 *
                 * So we find 3 that represent index in sequence[k].
                 */
                int index = sequence[dimension].IndexOf(value);
                
                /*
                 * This is an extension function for BitArray.
                 * This function write value into BitArray with lenght bitLength and offset.
                 * Look at this function documentation to find more.
                 */
                chromosome.WriteInt(index, offset, bitLength);
                
                /*
                 * Here we move our offset to write new coordinate into chomosome.
                 */
                offset += bitLength;
            }
            
            double fitness = GetFitness(pointVector);
            double coefficient = GetCoefficient(fitness);
            
            _phenotypes.Add(new Phenotype
            {
                Vector = pointVector,
                Coefficient = coefficient,
                FunctionValue = fitness,
                Chromosome = chromosome,
            });
        }
        _sequence = sequence;
        _bitLengths = bitLengths;
    }

    private List<List<double>> GetNewVector(List<List<double>> sequences)
    {
        IEnumerable<List<double>> result = new[] { new List<double>() };

        foreach (var sequence in sequences)
        {
            result = result.SelectMany(
                acc => sequence,
                (acc, item) => new List<double>(acc) { item }
            );
        }

        return result.ToList();
    }
    
    public abstract double GetFitness(List<double> point);
    public abstract double GetCoefficient(double fitness);
}


public static class BitArrayExtensions
{
    /// <summary>
    /// Writes an integer value (index) as a fixed-length binary block into an existing <see cref="BitArray"/>, 
    /// starting at the specified offset.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method uses the bitwise operation <c>(value &amp; (1 &lt;&lt; i))</c> to extract bits 
    /// from <paramref name="value"/> sequentially (from least significant to most significant) 
    /// and stores them in <paramref name="array"/>.
    /// </para>
    /// <para>
    /// <b>Execution flow:</b>
    /// <list type="bullet">
    ///   <item>
    ///     <description>Iteration <c>i = 0</c>: Checks the lowest bit (LSB) of <paramref name="value"/> and writes it to <c>array[offset]</c>.</description>
    ///   </item>
    ///   <item>
    ///     <description>Iteration <c>i = 1</c>: Checks the second bit and writes it to <c>array[offset + 1]</c>, repeating up to <c>bitLength - 1</c>.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <param name="array">The target <see cref="BitArray"/> instance (chromosome) to write into.</param>
    /// <param name="value">The integer value (coordinate index) to encode into binary.</param>
    /// <param name="offset">The starting bit index in <paramref name="array"/> where writing begins.</param>
    /// <param name="bitLength">The number of bits (<c>k</c>) allocated for this value inside the chromosome.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> or <paramref name="bitLength"/> is negative, 
    /// or when <c>offset + bitLength</c> exceeds the total length of <paramref name="array"/>.
    /// </exception>
    /// <example>
    /// Example of writing two coordinate indices into a shared <see cref="BitArray"/>:
    /// <code>
    /// // Create a chromosome with a total length of 5 bits:
    /// // X dimension uses 2 bits, Y dimension uses 3 bits.
    /// var chromosome = new BitArray(5); 
    /// 
    /// int xIndex = 2; // Binary (2 bits): '10' (LSB first: false, true)
    /// int yIndex = 4; // Binary (3 bits): '100' (LSB first: false, false, true)
    /// 
    /// // 1. Write X index into bits [0..1]: offset = 0, bitLength = 2
    /// chromosome.WriteInt(xIndex, offset: 0, bitLength: 2);
    /// 
    /// // 2. Write Y index into bits [2..4]: offset = 2, bitLength = 3
    /// chromosome.WriteInt(yIndex, offset: 2, bitLength: 3);
    /// 
    /// // Resulting bit sequence in chromosome:
    /// // Index 0: false (X bit 0)
    /// // Index 1: true  (X bit 1)
    /// // Index 2: false (Y bit 0)
    /// // Index 3: false (Y bit 1)
    /// // Index 4: true  (Y bit 2)
    /// </code>
    /// </example>
    public static void WriteInt(this BitArray array, int value, int offset, int bitLength)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (offset < 0 || bitLength < 0 || offset + bitLength > array.Length)
        {
            throw new ArgumentOutOfRangeException(
                $"Parameters {nameof(offset)} ({offset}) and {nameof(bitLength)} ({bitLength}) " +
                $"exceed the target BitArray capacity ({array.Length}).");
        }

        for (int i = 0; i < bitLength; i++)
        {
            // The mask (1 << i) isolates the i-th bit of value.
            // The boolean result is stored at position (offset + i).
            array.Set(offset + i, (value & (1 << i)) != 0);
        }
    }

    /// <summary>
    /// Reads a fixed-length binary block from a <see cref="BitArray"/> starting at the specified offset
    /// and converts it back into an integer value (index).
    /// </summary>
    /// <param name="array">The source <see cref="BitArray"/> (chromosome) to read from.</param>
    /// <param name="offset">The starting bit index in <paramref name="array"/>.</param>
    /// <param name="bitLength">The number of bits (<c>k</c>) to read for this value.</param>
    /// <returns>The decoded integer index.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> or <paramref name="bitLength"/> is negative, 
    /// or when <c>offset + bitLength</c> exceeds the target <see cref="BitArray"/> capacity.
    /// </exception>
    private static int ReadInt(this BitArray array, int offset, int bitLength)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (offset < 0 || bitLength < 0 || offset + bitLength > array.Length)
        {
            throw new ArgumentOutOfRangeException(
                $"Parameters {nameof(offset)} ({offset}) and {nameof(bitLength)} ({bitLength}) " +
                $"exceed the target BitArray capacity ({array.Length}).");
        }

        int value = 0;

        for (int i = 0; i < bitLength; i++)
        {
            if (array.Get(offset + i))
            {
                value |= (1 << i);
            }
        }

        return value;
    }
    
    /// <summary>
    /// Decodes a binary chromosome (<see cref="BitArray"/>) back into a multi-dimensional vector (<see cref="List{Double}"/>).
    /// </summary>
    /// <param name="chromosome">The chromosome containing concatenated binary representations of coordinate indices.</param>
    /// <param name="sequence">The pre-calculated discrete coordinate values for each dimension.</param>
    /// <param name="bitLengths">The number of bits allocated for each dimension.</param>
    /// <returns>A list of continuous double values representing the spatial coordinates (x, y, z...).</returns>
    public static  List<double> DecodeChromosome(this BitArray chromosome, List<List<double>> sequence, List<int> bitLengths)
    {
        ArgumentNullException.ThrowIfNull(chromosome);
        ArgumentNullException.ThrowIfNull(sequence);
        ArgumentNullException.ThrowIfNull(bitLengths);

        var vector = new List<double>(sequence.Count);
        
        /*
         * This variable contains offset for decoding.
         * In the offset we add the bit length of each dimension through each iteration.
         * For example:
         *          chromosome = | 0 1 0 0 | | 1 0 | | 1 1 1 | - each part represent its dimension.
         *      So we have 3 dimensions and we iterate 3 times.
         *          bitLength = bitLengths[dimension] = 3 - because this part (| 1 1 1 |) has 3 bits.
         *          index = ReadInt(currentOffset, bitLength) - This index represent certain value at sequence array.
         *      Remember, sequence contains all possible values at each dimension.
         * 
         * So when we get index we can now take certain value from sequence and build result vector. 
         */
        int currentOffset = 0;
        for (int dimension = 0; dimension < sequence.Count; dimension++)
        {
            int bitLength = bitLengths[dimension];

            int index = chromosome.ReadInt(currentOffset, bitLength);
            var currentSequence = sequence[dimension];
            
            if (index >= currentSequence.Count)
            {
                index = currentSequence.Count - 1;
            }

            double coordinateValue = currentSequence[index];
            vector.Add(coordinateValue);

            currentOffset += bitLength;
        }

        return vector;
    }
}