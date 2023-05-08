/* Copyright (C) 2023 Tomat & Contributors
 * 
 * Licensed under the GNU Lesser General Public License, version 2.1; you may
 * not use this file except in compliance with the License.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using Microsoft.Build.Utilities;

namespace Void.Build.NuGet.Shared;

/// <summary>
///     The context of a <see cref="VoidTask"/>.
/// </summary>
public sealed class VoidContext {
    /// <summary>
    ///     Gets or sets whether the task should catch exceptions.
    /// </summary>
    public bool CatchOnException { get; set; } = true;
}

/// <summary>
///     A simple abstraction over <see cref="Task"/> with rudimentary exception
///     handling.
/// </summary>
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

    /// <summary>
    ///     Executes the task.
    /// </summary>
    /// <param name="ctx">The context of the task.</param>
    /// <returns>
    ///     <see langword="true"/> if the task succeeded, <see langword="false"/>
    ///     otherwise.
    /// </returns>
    protected abstract bool Execute(VoidContext ctx);
}
