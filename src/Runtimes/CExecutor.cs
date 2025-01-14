using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

[ExecutorsAttribute("clang")]
public class CExecutor : IRuntimeExecutor
{

    public CExecutor()
    {
    }

    public ICollection<RuntimeDTO> Execute(RuntimeRequest request)
    {
        return null;
    }
}
