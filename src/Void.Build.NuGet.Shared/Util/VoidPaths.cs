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

using System.IO;

namespace Void.Build.NuGet.Shared.Util;

public static class VoidPaths {
    public const string VOID = ".void";
    public const string CACHE = "cache";
    public const string ASSEMBLIES = "assemblies";
    public const string ASSEMBLIES_CACHE_DIR_MANIFEST = "assemblies.json";

    public static readonly string ASSEMBLY_CACHE_DIR = Path.Combine(VOID, CACHE, ASSEMBLIES);
    public static readonly string ASSEMBLY_CACHE_MANIFEST_PATH = Path.Combine(ASSEMBLY_CACHE_DIR, ASSEMBLIES_CACHE_DIR_MANIFEST);
}
