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

using Mono.Cecil;
using Void.Build.NuGet.Shared.Models;

namespace Void.Build.NuGet.AssemblyPublicizer;

/// <summary>
///     Performs modifications to assemblies.
/// </summary>
public static class AssemblyModifier {
    public static AssemblyDefinition ModifyAssembly(string assemblyPath, IAssemblyResolver resolver, AssemblyCacheSettings settings) {
        var assembly = AssemblyDefinition.ReadAssembly(
            assemblyPath,
            new ReaderParameters {
                AssemblyResolver = resolver,
            }
        );

        if (settings.HasFlag(AssemblyCacheSettings.Publicized))
            PublicizeAssembly(assembly);

        return assembly;
    }

    private static void PublicizeAssembly(AssemblyDefinition assembly) {
        static void publicizeType(TypeDefinition type) {
            type.IsPublic = true;

            // Events also encompass properties and events. Yippee.
            foreach (var method in type.Methods)
                method.IsPublic = true;

            foreach (var field in type.Fields)
                field.IsPublic = true;

            foreach (var nested in type.NestedTypes)
                publicizeType(nested);
        }

        foreach (var type in assembly.MainModule.Types)
            publicizeType(type);
    }
}
