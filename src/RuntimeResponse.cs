using System.Collections.Generic;

public record RuntimeDTO(string Stdout, string Stderr, int ExitCode, float RunTime, string RunRAM);