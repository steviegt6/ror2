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

namespace Void.Optimizer.Core;

public interface IAllowanceProvider {
    bool IsAssemblyAllowed(
        AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    );

    bool IsTypeAllowed(
        TypeDefinition typeDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    );
}

public sealed class DefaultAllowanceProvider : IAllowanceProvider {
    public bool IsAssemblyAllowed(
        AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    ) {
        var items = assemblyWhitelist.Items;
        var asmName = assemblyDefinition.Name.Name;

        return assemblyWhitelist.AllowanceType switch {
            AllowanceType.Whitelist => items.Contains(asmName),
            AllowanceType.Blacklist => !items.Contains(asmName),
            AllowanceType.None => true,
            _ => false,
        };
    }

    public bool IsTypeAllowed(
        TypeDefinition typeDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    ) {
        var items = typeWhitelist.Items;
        var typeName = typeDefinition.FullName;

        return typeWhitelist.AllowanceType switch {
            AllowanceType.Whitelist => items.Contains(typeName),
            AllowanceType.Blacklist => !items.Contains(typeName),
            AllowanceType.None => true,
            _ => false,
        };
    }
}
