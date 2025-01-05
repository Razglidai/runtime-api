using System.Collections.Generic;

public record RuntimeResponse(string Runtime, string UUID, Dictionary<string, string> Output);