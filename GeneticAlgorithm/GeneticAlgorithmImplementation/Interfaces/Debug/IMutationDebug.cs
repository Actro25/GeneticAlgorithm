using System.Collections;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces.Debug;

public interface IMutationDebug
{
    public record MutationLogRecord(
        int PhenotypeId,
        BitArray OldChromosome,
        BitArray NewChromosome
    );

    void PrintMutationLog(List<MutationLogRecord> logs);
} 