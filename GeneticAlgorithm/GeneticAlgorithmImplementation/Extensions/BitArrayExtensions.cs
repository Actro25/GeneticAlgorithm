using System.Collections;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Extensions;

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