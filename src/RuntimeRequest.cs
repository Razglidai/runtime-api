using System.Collections.Generic;

public record RuntimeRequest(string Code, string UUID, Dictionary<string, string> Input);