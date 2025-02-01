using System.Collections.Generic;

public record RuntimeDTO(string stdout, string stderr, int exitCode, double runTime, long runRAM);