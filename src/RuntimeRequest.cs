using System.Collections.Generic;

public record RuntimeRequest(string Code, string UUID, ICollection<string> Input);