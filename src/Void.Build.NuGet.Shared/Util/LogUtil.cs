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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Utilities;

namespace Void.Build.NuGet.Shared.Util;

/// <summary>
///     Utilities for logging.
/// </summary>
public static class LogUtil {
    /// <summary>
    ///     Logs an enumerable collection of items.
    /// </summary>
    /// <param name="log">The <see cref="TaskLoggingHelper"/> to use for logging.</param>
    /// <param name="whenNonZero">
    ///     The message to log when the enumerable collection is not empty.
    /// </param>
    /// <param name="whenZero">
    ///     The message to log when the enumerable collection is empty.
    /// </param>
    /// <param name="items">The enumerable collection of items to log.</param>
    /// <typeparam name="T">
    ///     The type of the items in the enumerable collection.
    /// </typeparam>
    public static void LogEnumerable<T>(this TaskLoggingHelper log, string whenNonZero, string whenZero, IEnumerable<T> items) {
        var itemArray = items.ToArray();

        if (itemArray.Length == 0) {
            log.LogMessage(whenZero);
        }
        else {
            var padding = new string(' ', whenNonZero.Length);

            for (var i = 0; i < itemArray.Length; i++) {
                if (i == 0)
                    log.LogMessage(whenNonZero + itemArray[i]);
                else
                    log.LogMessage(padding + itemArray[i]);
            }
        }
    }
}
