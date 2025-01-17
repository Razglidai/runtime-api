using System.Collections.Generic;

public record RuntimeRequest(string code, ICollection<string> input);