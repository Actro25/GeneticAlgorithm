using System.Collections;

namespace GeneticAlgorithm.GeneticAlgorithmImplementation.Interfaces;

public interface IMutationDebug
{
    public record MutationLogRecord(
        int PhenotypeId,
        BitArray OldChromosome,
        BitArray NewChromosome
    );

    void PrintMutationTable(List<MutationLogRecord> logs);
} 