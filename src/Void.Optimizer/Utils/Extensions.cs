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

using System.Linq;
using Mono.Cecil;

namespace Void.Optimizer.Utils;

public static class Extensions {
    public static TypeDefinition? GetType(
        this AssemblyDefinition assemblyDefinition,
        string typeName
    ) {
        return assemblyDefinition.MainModule.GetType(typeName);
    }
    
    // extension to get method from TypeDefinition
    public static MethodDefinition? GetMethod(
        this TypeDefinition typeDefinition,
        string methodName
    ) {
        return typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);
    }
}
