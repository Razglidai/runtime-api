using System.Collections.Generic;

public record RuntimeResponse(string Runtime, string UUID, ICollection<string> Output);