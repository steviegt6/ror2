using System;
using Microsoft.Build.Utilities;

namespace Void.Build.NuGet.Shared.Tasks;

public sealed class VoidContext {
    public bool CatchOnException { get; set; } = true;
}

public abstract class VoidTask : Task {
    public sealed override bool Execute() {
        var ctx = new VoidContext();

        try {
            return Execute(ctx);
        }
        catch (Exception ex) {
            if (!ctx.CatchOnException)
                throw;

            Log.LogErrorFromException(ex);
            return false;
        }
    }

    protected abstract bool Execute(VoidContext ctx);
}
