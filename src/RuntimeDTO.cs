using System.Collections.Generic;

public record RuntimeDTO(string stdout, string stderr, int exitCode, long runTime, long runRAM);