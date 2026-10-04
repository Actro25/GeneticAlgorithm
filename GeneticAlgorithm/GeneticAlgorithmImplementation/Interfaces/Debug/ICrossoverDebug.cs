using System.Collections;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

public interface ICrossoverDebug
{
    public record CrossoverLogRecord(
        int ChunkId,
        int PointOfChange,
        List<BitArray> OldChromosome,
        List<BitArray> NewChromosome
    );
    
    void PrintCrossoverLog(List<CrossoverLogRecord> logs);
}